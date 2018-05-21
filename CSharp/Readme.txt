QuadPay Coding Assignment

QuadPay is a payment gateway based around splitting purchases into 4 interest free installments, every two weeks. The first 25% is taken when the order is received, and the remaining 3 installments of 25% are automatically taken every 14 days. We help customers manage their cash-flow and we help merchants increase conversion rates and average order values.

In the future, we'd like to be able to offer flexible installment schedules to our customers to help them better manage their cash flow. For example, we might want to let customers choose both the number of Installments their purchase is split into as well as the timespan between each installment.

We've encountered many gotchas and edge cases while implementing this platform and know there will be many more if we introduce variable installment counts and intervals. 

This coding assignment is designed to help you get familiar with our problem domain and start to think about what sort of scenarios we should anticipate going forward. It also gives us an opportunity to see how you would approach handling some of the edge cases we've already encountered.

There are two projects in this solution:

 - QuadPay.Domain contains a few domain objects for you to work with. You'll notice some of the necessary implementation is stubbed out and marked with TODOs. There may be some methods or properties missing, feel free to add more as you see fit.

 - QuadPay.Test contains a few example xUnit Unit Tests (all failing). 
 
 See if you can get the existing tests passing by fleshing out the missing PaymentPlan implementations. Also make sure to add new tests to increase coverage and ensure each test is properly checking all edge cases/gotchas you can think of.

--------------------------

If you're feeling ambitious and have extra time, put some thought into what other questions we might ask of our Domain model and what other functionality might be required in a PaymentPlan that does not exist in this version and have a go with implementing it.

Example 1: 
Customer Rose Ortiz has multiple PaymentPlans (Orders) with varying statuses: 
    How would you calculate Rose's on-time payment ratio so we can decide if we want to let Rose make more purchases with QuadPay? (Feel free to implement this as a Service)
    Is there a better way to model this domain to make this calculation more straightforward? 
    What if we wanted to calculate Rose's Current exposure (sum of all Outstanding Installment amounts)?

Example 2: 
Customer Alan Delgado is having trouble keeping up with his current PaymentPlan (e.g. 25% every 2 weeks). We'd like to be able to offer Alan a new PaymentPlan with a schedule that better fits his pay schedule and cash flow (e.g. 15% every 4 weeks). 
    What would be your approach to implementing support for this? 
    Would you create new Installments in the existing PaymentPlan?  
    Create a new PaymentPlan? 
Try to anticipate what sort of issues we might encounter (e.g. after the migration and a few new 15% payments are taken, the Merchant for Alan's order issues a refund for 70% of the original order amount after he returns part of his order).