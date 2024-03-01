using FastEndpoints;
using FluentValidation;

namespace ModifyResponse;

public class EndpointWithValidation<TRequest, TResponse, TValidation> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
{

}

public class PostValidationProcessor : IGlobalPostProcessor
{
    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        if (!context.HttpContext.ResponseStarted())
        {
            if (context.Response is BaseResponse response)
            {
                response.Validations.Add(new BaseValidation() { FieldId = "hello", Validation = "min:1" });
            }
        }

        return Task.CompletedTask;
    }
}

public class BaseResponse
{
    public object Fields { get; set; }

    public List<BaseValidation> Validations { get; set; } = [];
}

public class BaseValidation
{
    public required string FieldId { get; set; }

    public required string Validation { get; set; }
}

public class TestEndpointRequest
{
    public string Name { get; set; }
}

public class TestEndpointResponse : BaseResponse
{
    public new TestEndpointFields Fields { get; set; }

    public List<BaseValidation> Validations { get; set; } = [];
}

public class TestEndpointFields
{
    public string Hello { get; set; }
}

public class TestEndpointValidator : Validator<TestEndpointRequest>
{
    public TestEndpointValidator()
    {
        RuleFor(x => x.Name).NotNull().MinimumLength(1).MaximumLength(10);
    }
}

public class TestEndpoint : EndpointWithValidation<TestEndpointRequest, TestEndpointResponse, TestEndpointValidator>
{
    public override void Configure()
    {
        DontAutoSendResponse();
        AllowAnonymous();
        Get("/test/{Name}");
    }

    public override async Task HandleAsync(TestEndpointRequest req, CancellationToken ct)
    {
        await SendOkAsync(new TestEndpointResponse()
        {
            Fields = new TestEndpointFields()
            {
                Hello = req.Name
            }
        });
    }
}