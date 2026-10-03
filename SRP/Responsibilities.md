# SRP Violations

## 1. AppointmentDesk
- **Scheduling/Booking:** Keeps track of booked slots and finds available ones.
- **Business Rules:** Defines clinic opening hours and working days.
- **Calendar Export:** Formats appointment data into ICS format.
- **Messaging:** Formats the text for SMS reminders.

**Problem:** A change to the SMS text or calendar format could accidentally introduce bugs into the core scheduling logic or business rules.

## 2. CheckoutBasket
- **Cart Management:** Keeping track of items and calculating the base subtotal.
- **Marketing/Coupon Parsing:** Parsing marketing string codes like "SAVE10" or "FREESHIP".
- **Gift Wrap Policy:** Adding the $4.99 fee for gift wrapping.
- **Message Formatting:** Formatting the customer-facing "gift message card" text.
- **Payment Authorization:** Creating the faux payment gateway request string/hash.

**Problem:** A change in how coupons are parsed, or updating the gift message format, could break the actual cart totals or payment authorization logic.

## 3. CourseEnrollmentDesk
- **Enrollment/Seat Management:** Keeping track of registered and waitlisted students (`Register`, `WaitlistPosition`, `PromoteFromWaitlist`).
- **Student Communication:** Formatting the welcome markdown packet with onboarding links.
- **Invoicing/Finance:** Calculating VAT tax and formatting an invoice string.

**Problem:** A change to VAT rates or the Discord onboarding link could inadvertently introduce bugs into the waitlist promotion algorithm.

## 4. GradeBook
- **Score Aggregation:** Storing scores and calculating the math average (`Record`, `Average`).
- **Academic Policy:** Determining letter grade bands and honor roll rules (`Letter`, `MeetsHonorRoll`).
- **Data Export/Formatting:** Formatting text for registrar transcripts and CSV exports (`TranscriptPlain`, `ExportCsv`).

**Problem:** A change in the CSV export format or transcript layout could accidentally break the grading logic or honor roll calculation.

## 5. KitchenTicket
- **Order Management:** Managing the list of requested items (`_items` and `AddItem`).
- **Allergen Detection:** The logic and dictionary for mapping ingredients to allergens (`DetectAllergens`).
- **Kitchen Operations/Timing:** Estimating ready times and routing to expo lanes based on kitchen heuristics (`EstimatedReadyMinutes`, `ExpoLaneHint`).
- **Ticket Formatting/Printing:** Formatting the order for a physical thermal printer (`RenderThermalTicket`).

**Problem:** A change in the thermal printer width or vendor could unintentionally cause a bug in how long it takes to estimate an order.

## 6. LoanDesk
- **Risk Assessment/Underwriting:** The math formula that calculates the risk score and basic eligibility limits (`RiskScore`, `IsEligible`).
- **Compliance Checklist:** The rules mapping loan properties to required documents (`RequiredDocuments`).
- **Letter Formatting/Comms:** The prose and wording for the decision letter sent to applicants (`DecisionLetter`).
- **Analytics Export:** Formatting the data into a CSV row for underwriters/analytics (`UnderwriterCsvRow`).

**Problem:** A change in the wording of a rejection letter shouldn't require testing the complex financial risk math again.

## 7. SubscriptionBilling
- **Proration/Pricing Math:** Calculating how much to charge based on days used (`Prorate`).
- **Invoice Number Generation:** Formatting and minting the next invoice ID (`NextInvoiceNumber`).
- **Payment Status Tracking:** Keeping track of the number of failed payments (`RegisterFailedPayment`).
- **Customer Communication/Collections:** Drafting the dunning email text to send to customers (`DunningEmail`).
- **Accounting Export:** Formatting a ledger row for the accounting system (`LedgerJournalLine`).

**Problem:** A change to the dunning email text could accidentally introduce a bug into the pricing math.

## 8. SupportTicket
- **Ticket State Management:** Holding the ticket ID, subject, and body, and adding new messages (`AppendCustomerMessage`).
- **Keyword Scanning (NLP):** Scanning text to guess Priority/Urgency (`RecalculatePriorityFromText`).
- **SLA Policy:** The rules translating a Priority level into a mathematical time deadline (`SlaDeadline`, `IsBreached`).
- **Public Communication:** Creating the text template sent to the customer (`DraftPublicReply`).
- **Internal Logging:** Formatting a tiny blurb meant for internal Slack/Discord escalation (`InternalEscalationBlurb`).

**Problem:** A change to the keyword "outage" should not accidentally alter the mathematical calculation of a 4-hour SLA deadline.

## 9. WardBoard
- **Bed Tracking:** Managing which patient is assigned to which bed (`AssignBed`).
- **Clinical Acuity Policy:** The medical math that translates heart rate and SPO2 into a clinical score (`ScoreAcuity`).
- **Escalation/Paging Rules:** The policy that fires a "CODE-YELLOW" pager alert when a score hits 8 (`_pagerLog`, `DrainPagerLog`).
- **Handoff Formatting:** Formatting the prose for nurse handoff notes (`BuildHandoffNote`).
- **Census Export:** Formatting the data as a CSV for export (`ExportCensusCsv`).

**Problem:** A change to the medical acuity formula shouldn't accidentally affect the CSV formatting used for the hospital's database exports.

## 10. WarehousePickList
- **Pick List Management:** Tracking the items that need to be picked (`AddNeed`).
- **Inventory Allocation Policy:** The logic deciding how much of an item can actually be given based on stock (`Allocate`).
- **Pathing/Routing Heuristics:** The sorting logic to determine the most efficient walking path (`WalkingOrder`).
- **Picker Instructions/UX:** Formatting the human-readable instructions for the handheld device (`PickerScript`).
- **WMS Integration:** Formatting the XML batch file for the Warehouse Management System (`WmsXmlBatch`).

**Problem:** A change in the XML format required by the integration shouldn't force us to re-test the walking path algorithms.
