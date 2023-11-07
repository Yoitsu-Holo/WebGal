using KristofferStrube.Blazor.WebAudio;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using WebGal;
using WebGal.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
	BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder.Services.AddScoped<GameManager>();
// builder.Services.AddScoped<Lazy<Task<AudioContext>>>(sp => new(async () =>
// {
// 	var js = sp.GetRequiredService<IJSRuntime>();
// 	return await AudioContext.CreateAsync(js);
// }));

await builder.Build().RunAsync();
