using Client.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Api.SwaggerExamples;

public class UpdateOrderExample : IExamplesProvider<UpdateOrderRequestDto>
{
    public UpdateOrderRequestDto GetExamples()
    {
        return new UpdateOrderRequestDto
        {
            OrderNumber = "<GET FROM CREATE>",
            ShippingAddress = new AddressDto
            {
                AddressLineOne = "123 Updated Shipping Line One",
                AddressLineTwo = "Updated Shipping Line Two",
                AddressLineThree = "Updated Shipping Line Three",
                PostCode = "M1 3DE"
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
                    Quantity = 1,
                    Sku = "L1700FSLH",
                },
                new()
                {
                    Quantity = 1,
                    Sku = "CRZ-PK",
                }
            },
        };
    }
}