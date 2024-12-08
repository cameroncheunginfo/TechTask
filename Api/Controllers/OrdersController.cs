using Api.SwaggerExamples;
using Application.Exceptions;
using Application.Orders;
using Client.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderReader _orderReader;
    private readonly IOrderCreator _orderCreator;
    private readonly IOrderUpdater _orderUpdater;

    public OrdersController(IOrderReader orderReader,
                            IOrderCreator orderCreator,
                            IOrderUpdater orderUpdater)
    {
        _orderReader = orderReader;
        _orderCreator = orderCreator;
        _orderUpdater = orderUpdater;
    }

    [HttpGet("[action]")]
    public Task<IActionResult> Get([FromQuery] string orderNumber)
    {
        try
        {
            var response = _orderReader.ReadOrder(orderNumber);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        catch (ValidationException ex)
        {
            // Consider using the ProblemDetails class to return a standardised detailed error messages (https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails?view=aspnetcore-8.0)
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            /* Should not be throwing base exceptions (https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/exceptions/creating-and-throwing-exceptions#things-to-avoid-when-throwing-exceptions).
             * You should not be throwing a new exception as by doing this you will lose the stack stack trace of the initial exception.
             * You should not be exposing the stack trace to consumers of the API. Just return a 500 with a generic error message
             * You could add some logging here to log the exception.
             */
            throw new Exception(ex.Message);
        }
    }


    //If two requests come in for the same SKU with only 1 in stock then both orders will be "Created" - do you not need to check stock levels and modify them before saying the order is accepted?
    // No check for whatever you're ordering is in stock - unless this in done in a earlier step in the order placement flow
    [HttpPost("[action]")]
    [SwaggerRequestExample(typeof(CreateOrderRequestDto), typeof(CreateOrderExample))]
    public Task<IActionResult> Create([FromBody] CreateOrderRequestDto request)
    {
        try
        {
            var response = _orderCreator.CreateOrder(request);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        catch (ValidationException ex)
        {
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPut("[action]")]
    [SwaggerRequestExample(typeof(UpdateOrderRequestDto), typeof(UpdateOrderExample))]
    public Task<IActionResult> Update([FromBody] UpdateOrderRequestDto request)
    {
        try
        {
            var response = _orderUpdater.UpdateOrder(request);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        catch (ValidationException ex)
        {
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
