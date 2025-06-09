// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************
using NCalc;
using System.Diagnostics;
using Xunit; 

namespace Test01NCalc;

class Product(string name, string code, decimal price)
{
    public string Name { get; set; } = name;
    public string Code { get; set; } = code;
    public decimal Discount { get; set; }
    public decimal Price { get; set; } = price;
    public decimal BultoPrice { get; set; }  
    public string? Weight { get; set; }
    public string? Unit { get; set; }
}

public class ProductTests
{
    [Fact]
    public void Product_Constructor_SetsProperties()
    {
        var product = new Product("*Coca cola", "001", 1.55m);
        var expression = new Expression("2 + 3 * 5");
        Assert.Equal(17 ,(int) expression.Evaluate());

        expression = new Expression("if(product.Name LIKE '*%','','<有会员折扣>')");
        expression.Parameters["product"] = product;
        Assert.Equal("<有会员折扣>", expression.Evaluate());

    }
} 
