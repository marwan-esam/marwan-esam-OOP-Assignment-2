using System;
using System.Collections.Generic;
using SrpLab;

Console.WriteLine("SrpLab — 10 intentional SRP violations (refactored)");
Console.WriteLine("=================================================");

var bedTracker = new BedTracker();
var acuityPolicy = new ClinicalAcuityPolicy();
var pagerPolicy = new PagerEscalationPolicy();
var handoffFormatter = new HandoffNoteFormatter();

bedTracker.AssignBed(1, "p-88");
var hr = 130;
var spo2 = 89;
var acuity = acuityPolicy.ScoreAcuity(hr, spo2);
pagerPolicy.EvaluateAcuity(1, acuity);

Console.WriteLine(handoffFormatter.BuildHandoffNote(1, bedTracker.GetPatient(1), acuity));
Console.WriteLine(string.Join(" | ", pagerPolicy.DrainPagerLog()));

var basket = new CheckoutBasket();
basket.AddLine("SKU-1", 40m, 2);
var couponParser = new CouponParser();
var discount = couponParser.DiscountAmount("SAVE10", basket.SubTotal());
var wrapPolicy = new GiftWrapPolicy();
var fee = wrapPolicy.CalculateGiftWrapFee(true);
var grandTotal = Math.Max(0m, basket.SubTotal() - discount + fee);
var gateway = new PaymentGatewayStub();

Console.WriteLine($"basket total={grandTotal} auth={gateway.AuthorizePaymentStub(grandTotal, "4242", basket.Lines.Count)}");

var ticketData = new TicketData("T-1", "cannot login", "prod is down for me", DateTimeOffset.UtcNow);
var priorityScanner = new PriorityScanner();
var priority = priorityScanner.RecalculatePriorityFromText(ticketData);
var slaPolicy = new SlaPolicy();
var deadline = slaPolicy.SlaDeadline(ticketData.OpenedAt, priority);
var publicReply = new PublicReplyFormatter();

Console.WriteLine(publicReply.DraftPublicReply("Nora", ticketData.Id, priority, deadline));

var loanApp = new LoanApplication(60_000m, 640, 4, hasCollateral: false);
var riskAssessor = new RiskAssessor();
var riskScore = riskAssessor.RiskScore(loanApp);
var isEligible = riskAssessor.IsEligible(riskScore, loanApp.CreditScore);
var compliance = new ComplianceChecklist();
var docs = compliance.RequiredDocuments(loanApp, isEligible);
var letterFormatter = new DecisionLetterFormatter();

Console.WriteLine(letterFormatter.DecisionLetter("Omar", loanApp.RequestedAmount, riskScore, isEligible, docs));

var courseDesk = new CourseEnrollmentDesk("SEF-101", 1);
Console.WriteLine(courseDesk.Register("a@mail.com"));
Console.WriteLine(courseDesk.Register("b@mail.com"));
var welcomeFormatter = new WelcomePacketFormatter();
Console.WriteLine(welcomeFormatter.WelcomePacketMarkdown("SEF-101", "b@mail.com", "Bea", courseDesk));

var kitchenOrder = new KitchenOrder();
kitchenOrder.AddItem("Pasta", new[] { "wheat", "milk" }, 12);
var allergenDetector = new AllergenDetector();
var allergens = allergenDetector.DetectAllergens(kitchenOrder);
var timingHeuristics = new KitchenTimingHeuristics();
var eta = timingHeuristics.EstimatedReadyMinutes(kitchenOrder, 2, allergens.Count);
var ticketFormatter = new ThermalTicketFormatter();
Console.WriteLine(ticketFormatter.RenderThermalTicket(42, kitchenOrder, eta, allergens));

var subProration = new SubscriptionProration(99m, new DateOnly(2026, 9, 1), new DateOnly(2026, 10, 1));
var tracker = new PaymentStatusTracker();
tracker.RegisterFailedPayment();
var amount = subProration.Prorate(new DateOnly(2026, 9, 1));
var invoiceNumber = InvoiceNumberGenerator.NextInvoiceNumber(subProration.PeriodStart);
var dunningFormatter = new DunningEmailFormatter();
Console.WriteLine(dunningFormatter.DunningEmail("Sara", new DateOnly(2026, 9, 20), tracker.FailedPayments, amount, invoiceNumber));

var pickTracker = new PickListTracker();
pickTracker.AddNeed("BOLT", "A", 3, 10, 7);
pickTracker.AddNeed("NUT", "B", 1, 5, 5);
var routing = new RoutingHeuristics();
var allocator = new InventoryAllocator();
var scriptFormatter = new PickerScriptFormatter();
Console.WriteLine(scriptFormatter.PickerScript(routing, allocator, pickTracker));

var grades = new GradeAggregator();
grades.Record("s1", 92);
grades.Record("s1", 88);
var academicPolicy = new AcademicPolicy();
var avg = grades.Average("s1");
var letter = academicPolicy.Letter(avg);
var honorRoll = academicPolicy.MeetsHonorRoll(avg);
var transcriptFormatter = new TranscriptFormatter();
Console.WriteLine(transcriptFormatter.TranscriptPlain("s1", "Ali", avg, letter, honorRoll));

var clinicPolicy = new ClinicSchedulePolicy(new TimeOnly(9, 0), new TimeOnly(17, 0), 30);
var scheduler = new AppointmentScheduler(clinicPolicy);
var slot = scheduler.FindNextSlot(DateTimeOffset.Parse("2026-09-21T08:00:00Z"), 48);
if (slot is null) throw new InvalidOperationException("no slot");
scheduler.TryBook(slot.Value);
Console.WriteLine(SmsReminderFormatter.SmsReminder(slot.Value, "0100"));

Console.WriteLine("Done. Responsibilities are split!");
