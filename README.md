## Additional notes

### Client.Dtos project

Folders could be added to better organised into presentation layer requests and DTOs.
There is also inconsistent naming between `CreateOrderRequest` and `UpdateOrderRequestDto`, which are both request but only one is suffixed with "Dto".

I think also there is also usually benefit to having seperate presentation layer models and application layer models, as the two can deviate. For example, for a endpoint that requires auth, you may fetch the user details from the auth token in presentation layer and pass it to the application layer, in which case the HTTP request model should not contain the user details and the application layer model will.

### Addresses

The addresses seem better suited as a value object. You can change Address from a class to a record and use EF Complex Types (https://devblogs.microsoft.com/dotnet/announcing-ef8-rc1/#complex-types-as-value-objects)

Issues:

As it currently is - due to the fact that multiple orders map to the same address, changing the shipping address for one order updates the address for all the other mapped orders.

This may also remove the need for the address hash comparison, unless it is needed for another business requirement.
