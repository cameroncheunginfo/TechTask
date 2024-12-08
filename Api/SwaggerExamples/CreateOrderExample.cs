using Client.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Api.SwaggerExamples;

public class CreateOrderExample : IExamplesProvider<CreateOrderRequestDto>
{
    public CreateOrderRequestDto GetExamples()
    {
        return new CreateOrderRequestDto
        {
            Customer = new CustomerDto
            {
                CustomerName = "Customer Name",
                EmailAddress = "customername@email.com",
                PhoneNumber = "07123 456 789",
                BillingAddress = new AddressDto
                {
                    AddressLineOne = "123 Billing Line One",
                    AddressLineTwo = "Billing Line Two",
                    AddressLineThree = "Billing Line Three",
                    PostCode = "M1 1AB"
                },
                ShippingAddress = new AddressDto
                {
                    AddressLineOne = "123 Shipping Line One",
                    AddressLineTwo = "Shipping Line Two",
                    AddressLineThree = "Shipping Line Three",
                    PostCode = "M1 3CD"
                }
            },
            OrderItems = new List<CreateOrderItemDto>
            {
                new ()
                {
                    Quantity = 1,
                    Sku = "BFRE011",
                },
                new()
                {
                    Quantity = 2,
                    Sku = "L1700FSLH",
                }
            },
        };
    }
}