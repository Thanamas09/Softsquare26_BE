using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Food_order_Backend.Filters;

public class TagOrderFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // กำหนดลำดับที่ต้องการ
        var tagOrder = new List<string>
        {
            "Users",
            "Orders",
            "Products",
            "Categories",
            "Dashboard"
        };

        // เรียง paths ตามลำดับ tag
        var orderedPaths = swaggerDoc.Paths
            .OrderBy(p =>
            {
                var tag = context.ApiDescriptions
                    .FirstOrDefault(a => $"/{a.RelativePath}" == p.Key || 
                                        $"/{a.RelativePath}".StartsWith(p.Key))
                    ?.ActionDescriptor.RouteValues["controller"];

                var index = tagOrder.IndexOf(tag ?? "");
                return index == -1 ? 999 : index;
            })
            .ToDictionary(p => p.Key, p => p.Value);

        swaggerDoc.Paths.Clear();
        foreach (var path in orderedPaths)
            swaggerDoc.Paths.Add(path.Key, path.Value);

        // เรียง Tags
        swaggerDoc.Tags = tagOrder
            .Select(t => new OpenApiTag { Name = t })
            .ToList();
    }
}
