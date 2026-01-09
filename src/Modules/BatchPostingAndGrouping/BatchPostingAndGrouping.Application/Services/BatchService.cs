using BatchPostingAndGrouping.Application.DTOs;
using BatchPostingAndGrouping.Domain.Entities;
using BatchPostingAndGrouping.Domain.Repositories;
using BatchPostingAndGrouping.Domain.ValueObjects;
using SharedKernel.Domain;

namespace BatchPostingAndGrouping.Application.Services;

public sealed class BatchService : IBatchService
{
    private readonly IBatchRepository _batchRepository;
    private readonly IFarmerProfileRepository _farmerProfileRepository;

    public BatchService(
        IBatchRepository batchRepository,
        IFarmerProfileRepository farmerProfileRepository)
    {
        _batchRepository = batchRepository;
        _farmerProfileRepository = farmerProfileRepository;
    }

    public async Task<BatchDto> PostBatchAsync(
        Guid farmerProfileId,
        PostBatchDto dto,
        CancellationToken cancellationToken = default)
    {
        // Ensure farmer profile exists
        var farmerProfile = await _farmerProfileRepository.GetByIdAsync(farmerProfileId, cancellationToken);
        if (farmerProfile == null)
            throw new DomainException($"Farmer profile with ID {farmerProfileId} not found.");

        // Create value objects
        var produceType = new ProduceType(dto.ProduceTypeName, dto.ProduceTypeCategory, dto.ProduceTypeUnit);
        var qualityGrade = new QualityGrade(dto.QualityGrade, dto.QualityGradeDescription);
        var location = new Location(dto.Latitude, dto.Longitude, dto.Address);

        // Create batch entity
        var batchId = Guid.NewGuid();
        var batch = new Batch(
            batchId,
            farmerProfileId,
            produceType,
            qualityGrade,
            dto.WeightInKg,
            location,
            dto.ReadyDateUtc,
            dto.PickupWindowEndUtc);

        // Save batch
        await _batchRepository.AddAsync(batch, cancellationToken);

        // Raise domain event (should be published by infrastructure layer after SaveChanges)
        var batchPostedEvent = batch.Post();

        return MapToDto(batch);
    }

    public async Task<BatchDto?> GetBatchByIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);
        return batch == null ? null : MapToDto(batch);
    }

    public async Task<IReadOnlyList<BatchDto>> GetBatchesByFarmerAsync(
        Guid farmerProfileId,
        CancellationToken cancellationToken = default)
    {
        var batches = await _batchRepository.GetByFarmerProfileIdAsync(farmerProfileId, cancellationToken);
        return batches.Select(MapToDto).ToList();
    }

    public async Task<BatchDto> UpdateBatchAsync(
        Guid batchId,
        Guid farmerProfileId,
        UpdateBatchDto dto,
        CancellationToken cancellationToken = default)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch == null)
            throw new DomainException($"Batch with ID {batchId} not found.");

        if (batch.FarmerProfileId != farmerProfileId)
            throw new DomainException("Cannot update batch that belongs to another farmer.");

        // Update produce type if provided
        if (dto.ProduceTypeName != null)
        {
            var produceType = new ProduceType(dto.ProduceTypeName, dto.ProduceTypeCategory);
            batch.UpdateProduceType(produceType);
        }

        // Update quality grade if provided
        if (dto.QualityGrade != null)
        {
            var qualityGrade = new QualityGrade(dto.QualityGrade);
            batch.UpdateQualityGrade(qualityGrade);
        }

        // Update weight if provided
        if (dto.WeightInKg.HasValue)
        {
            batch.UpdateWeight(dto.WeightInKg.Value);
        }

        // Update location if provided
        if (dto.Latitude.HasValue && dto.Longitude.HasValue)
        {
            var location = new Location(dto.Latitude.Value, dto.Longitude.Value, dto.Address);
            batch.UpdatePickupLocation(location);
        }

        // Update ready date if provided
        if (dto.ReadyDateUtc.HasValue)
        {
            batch.UpdateReadyDate(dto.ReadyDateUtc.Value, dto.PickupWindowEndUtc);
        }

        await _batchRepository.UpdateAsync(batch, cancellationToken);
        return MapToDto(batch);
    }

    public async Task CancelBatchAsync(
        Guid batchId,
        Guid farmerProfileId,
        CancelBatchDto dto,
        CancellationToken cancellationToken = default)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch == null)
            throw new DomainException($"Batch with ID {batchId} not found.");

        if (batch.FarmerProfileId != farmerProfileId)
            throw new DomainException("Cannot cancel batch that belongs to another farmer.");

        batch.Cancel(dto.Reason);
        await _batchRepository.UpdateAsync(batch, cancellationToken);
    }

    private static BatchDto MapToDto(Batch batch)
    {
        return new BatchDto
        {
            Id = batch.Id,
            FarmerProfileId = batch.FarmerProfileId,
            ProduceTypeName = batch.ProduceType.Name,
            ProduceTypeCategory = batch.ProduceType.Category,
            QualityGrade = batch.QualityGrade.Grade,
            WeightInKg = batch.WeightInKg,
            Latitude = batch.PickupLocation.Latitude,
            Longitude = batch.PickupLocation.Longitude,
            Address = batch.PickupLocation.Address,
            ReadyDateUtc = batch.ReadyDateUtc,
            PickupWindowEndUtc = batch.PickupWindowEndUtc,
            Status = batch.Status.ToString(),
            CreatedAtUtc = batch.CreatedAtUtc,
            UpdatedAtUtc = batch.UpdatedAtUtc
        };
    }
}
