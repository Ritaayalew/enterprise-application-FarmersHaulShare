| Full Name     | ID          |
|---------------|-------------|
| Rita Ayalew   | UGR/7639/15 |
| Hawi Mekonnen | UGR/5788/15 |
| Ruhama Ristu  | UGR/4614/15 |
| Lidia Asamnew | UGR/2984/15 |
| Ribka Mengiste| UGR/9680/15 |

# FarmersHaulShare: Cooperative Logistics Matcher for Smallholder Farmers

This idea aims to cut out exploitative middlemen by grouping nearby co-op members into shared “haul shares,” quoting fixed prices, and routing affordable trucks or moto-taxis to deliver fresh produce directly to cafes and restaurants in Addis. Farmers post batch (unit of produce that a farmer is ready to sell or deliver at a certain time) sizes and ready dates. The system clusters them by location and time, creates fair cost-sharing, and pushes delivery tracking updates. The goal is higher farmer revenue, predictable logistics for buyers, and efficient, transparent transport.

## Business Problem and Goals

Smallholder farmers around Addis Ababa struggle with:

- **Low bargaining power**: Middlemen dictate prices due to fragmented, small batches.
- **Inefficient transport**: Half-full trucks, last-minute arrangements, and opaque costs.
- **Demand mismatch**: Cafes/restaurants want consistent quality and timing, but farmers’ harvests vary.
- **Lack of coordination tools**: Existing solutions don’t group supply, quote fair shared costs, or provide simple tracking.

Farmers HaulShare addresses these by batching nearby supply, offering fixed-price shared haul quotes, coordinating transport via geolocation, and sending real-time updates. Success metrics: improved effective price per kg to farmers, higher truck utilization, on-time delivery rate, and reduced canceled orders.

## Subdomains

### Core Subdomain

1. **Cooperative Haul Orchestration**  
   **Business Value**: Creates the “haul share” by grouping supply, matching buyers, quoting fair shared transport costs, and scheduling pickup and delivery. This is the differentiator and primary source of value.

### Supporting Subdomains

1. **Transport Marketplace & Dispatch**  
   **Business Value**: Onboard drivers, verify vehicles, manages availability windows, and assigns jobs with turn-by-turn routing and geofencing. Supports core orchestration.

2. **Pricing & Revenue Sharing**  
   **Business Value**: Computes fixed haul prices, splits cost fairly across participants, applies surcharges or incentives, and settles payouts. Critical for trust; complements core orchestration.

### Generic Subdomains

1. **Identity & Access Management**  
   **Business Value**: Handles user registration, roles (farmer, buyer, driver, coordinator), authentication, and permissions. Commodity functionality.

2. **Messaging & Notifications**  
   **Business Value**: Template-driven notifications, delivery status updates, and reminders. Standardized comms infrastructure.

3. **Catalog & Contracts**  
   **Business Value**: Product definitions (e.g., tomatoes), buyer preferences, SLAs, and simple contracts.

## Core Domain Bounded Contexts

### Batch Posting and Grouping

- **Responsibility**: Farmers post batch size, grade, ready date/time, pickup location. System maintains farmer calendars and uses constraints (distance/time windows/quality) to form candidate groups. Supports edits, cancellations, and lock windows.
- **Key Concepts**: Batch, Availability window, Group candidate, Group lock, Farmer profile.

### Haul Share Creation and Scheduling

- **Responsibility**: Converts grouped supply into a “haul share” with target destination(s), pickup plan, delivery window, and service level. Validates volume-to-vehicle fit, reserves capacity, and issues fixed price offers.
- **Key Concepts**: HaulShare, Capacity plan, Pickup route, Delivery window, FixedPriceQuote, Offer acceptance.

### Pricing and Fair Cost Split

- **Responsibility**: Computes total haul price (base rate + distance + load factor + time-of-day + risk buffer), then splits fairly across participants (e.g., by weight, volume, or value-weighted share). Applies caps/floors and transparency receipts.
- **Key Concepts**: Tariff, PriceComponent, CostSplitLine, SettlementInstruction, TransparencyReceipt.

## Cross-Context User Stories and Event-Driven Interactions

### Story 1: Create a Haul Share from Posted Batches

- **Flow**: Multiple farmers post batches within overlapping windows. System groups them, validates capacity, and creates a haul share with a fixed-price quote for delivery to Addis.
- **Contexts Involved**: Batch Posting and Grouping; Haul Share Creation and Scheduling; Pricing and Fair Cost Split; Messaging and Notifications.
- **Domain Events**:
  - Published: `BatchPosted` (Batch Posting and Grouping)
  - Subscribed: `BatchPosted` → triggers `GroupCandidateFormed` (Batch Posting and Grouping)
  - Published: `GroupCandidateLocked` (Batch Posting and Grouping)
  - Subscribed: `GroupCandidateLocked` → `HaulShareCreated` (Haul Share Creation and Scheduling)
  - Published: `HaulShareCreated`
  - Subscribed: `HaulShareCreated` → `FixedPriceQuoteCalculated` (Pricing and Fair Cost Split)
  - Published: `FixedPriceQuoteCalculated`
  - Subscribed: `FixedPriceQuoteCalculated` → `QuoteSent` (Messaging and Notifications)
- **Eventual Consistency**: Asynchronous messaging ensures each context updates its projections independently. Group lock precedes haul share creation; pricing runs after haul share creation and publishes quote. Outbox pattern with a broker delivers events; read models reconcile within seconds.

### Story 2: Driver Acceptance and Pickup Tracking

- **Flow**: A verified driver sees the job, accepts, and begins pickup. Geofencing updates ETA and notifies farmers and buyers.
- **Contexts Involved**: Transport Marketplace & Dispatch; Haul Share Creation and Scheduling; Messaging and Notifications.
- **Domain Events**:
  - Published: `DriverAvailabilityUpdated` (Transport Marketplace & Dispatch)
  - Subscribed: `HaulShareCreated` → `DispatchJobPosted` (Transport Marketplace & Dispatch)
  - Published: `DispatchJobAccepted`
  - Subscribed: `DispatchJobAccepted` → `PickupRouteConfirmed` (Haul Share Creation and Scheduling)
  - Published: `GeofencePingReceived`; `PickupStarted`; `PickupCompleted`; `DeliveryStarted`; `DeliveryCompleted`
  - Subscribed: Status events → `StatusUpdateSent` (Messaging and Notifications)
- **Eventual Consistency**: GPS and geofence pings stream into Dispatch, which publishes status events. Scheduling consumes to update haul share timelines; Notifications push updates. Temporary divergences (e.g., delayed pings) resolve as events arrive.

### Story 3: Settlement and Transparent Cost Sharing

- **Flow**: After delivery completion, system finalizes the haul price and splits costs. Farmers get receipts showing their share, surcharges, and savings versus solo haul.
- **Contexts Involved**: Pricing and Fair Cost Split; Batch Posting and Grouping; Messaging and Notifications; Catalog & Contracts.
- **Domain Events**:
  - Published: `DeliveryCompleted` (Dispatch)
  - Subscribed: `DeliveryCompleted` → `SettlementCalculated` (Pricing and Fair Cost Split)
  - Published: `SettlementCalculated`; `TransparencyReceiptGenerated`
  - Subscribed: `TransparencyReceiptGenerated` → `ReceiptSent` (Messaging and Notifications)
  - Subscribed: `SettlementCalculated` → `BatchSettlementLinked` (Batch Posting and Grouping)
- **Eventual Consistency**: Settlement waits for `DeliveryCompleted`. Pricing uses immutable inputs (tariffs, distance, load factor) and publishes `SettlementCalculated`. Batch context links settlements to each farmer’s batch records via event consumption.

## AI-Driven Feature

**AI-Assisted Grouping with Dynamic Pricing Guidance and Demand Routing**

- **Purpose**: Improve batch clustering and price fairness while maintaining transparency.
- **Capabilities**:
  - **Smart Grouping**: A model predicts optimal groupings based on geo proximity, harvest readiness windows, product type, historical on-time performance, and vehicle fit. It maximizes truck utilization and minimizes total travel time.
  - **Fair Cost Suggestions**: The model recommends split strategies (weight-based vs. value-based), flagging outliers (e.g., a small batch far off-route) and proposing adjustments to keep shares equitable.
  - **Demand Routing Hints**: Predicts buyer demand spikes (e.g., weekend cafe orders) and suggests delivery windows and destinations that raise farmer revenue without violating constraints.
- **Domain Fit**: AI operates inside the Batch Posting and Grouping and Pricing contexts as decision support, not as a black box. Final quotes and splits are rule-based and auditable, with AI providing ranked suggestions and explanations to coordinators.

## Architecture and Consistency Approach

### Modular Monolith First

- **Module Boundaries**: Align with bounded contexts to isolate domain models, application services, and persistence per module.
- **Message Flow**: Use an in-process domain event bus plus the Outbox pattern to persist and publish events reliably to a broker.
- **Data Ownership**: Each module owns its tables; cross-module queries use projections/read models rather than joins.

### Microservices Later

- **Service Carving**: Promote each bounded context to an independent service with its own database.
- **Contracts**: Adopt schema-versioned event contracts and idempotent consumers.
- **Observability**: Correlate distributed flows via trace IDs on events; use retries with dead-letter queues for poison messages.

### Consistency Strategy

- **Eventual Consistency**: Embrace asynchronous events for cross-context workflows (e.g., `HaulShareCreated` → `FixedPriceQuoteCalculated`).
- **Compensation**: Define compensating actions for failures (e.g., cancel haul share if pricing fails; reassign driver if acceptance times out).
- **Concurrency Controls**: Use group locks and time-windowed reservations to prevent double-booking.
