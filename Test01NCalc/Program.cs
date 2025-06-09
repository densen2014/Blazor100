using NCalc;
using NCalc.Handlers;
using System.Reflection;
using Test01NCalc;

var product = new Product("*Coca cola", "001", 1.55m);
product.Discount = 10m; // 设置折扣属性
product.BultoPrice = 18.00m; // 设置批量价格属性 
product.Unit = "Kg"; // 设置单位属性

var expr = new Expression("2 + 3 * 5");
var res= expr.Evaluate();


EvaluateParameterHandler value = (string name, ParameterArgs args) =>
{
    // 只处理以 "product." 开头的参数
    if (name.StartsWith("product.", StringComparison.OrdinalIgnoreCase))
    {
        var propName = name.Substring("product.".Length);
        // 通过反射获取属性值
        var prop = typeof(Product).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop != null)
        {
            args.Result = prop.GetValue(product);
            return;
        }
    }
    // 其他参数处理
    if (name == "Pi")
        args.Result = 3.14;
};

void Expr_EvaluateFunction(string name, FunctionArgs args)
{
    if (name == "Weight")
    {
        // 计算重量
        if (product.BultoPrice >0)
        {
            args.Result = $"1 {(string.IsNullOrWhiteSpace(product.Unit)? "Litro/ 1 Kg" : product.Unit)} {product.BultoPrice} Euros";
        } 
    }else if (name == "VIP")
    {
        // 计算价格
        args.Result = product.Name.StartsWith('*')? "<有会员折扣>" : string.Empty;
    }
}

expr = new Expression("if([product.Name] LIKE '*%','<有会员折扣>','')", ExpressionOptions.StringConcat);
expr.EvaluateParameter += value;
res = expr.Evaluate();

Console.WriteLine(res); // 输出: <有会员折扣>

expr = new Expression("VIP()", ExpressionOptions.StringConcat);
expr.EvaluateFunction += Expr_EvaluateFunction;
res = expr.Evaluate();

Console.WriteLine(res); // 输出: <有会员折扣>

expr = new Expression("if([product.Discount] > 0, [product.Price] * [product.Discount], [product.Price])", ExpressionOptions.StringConcat);
expr.EvaluateParameter += value;
res = expr.Evaluate();
Console.WriteLine(res); // 输出: <折后价>

expr = new Expression("if([product.BultoPrice] > 0, '1 '  + if(([product.Unit]=='' || [product.Unit]==null),'Litro/ 1 Kg',[product.Unit]) + ' ' + [product.BultoPrice] + ' Euros', '')", ExpressionOptions.StringConcat | ExpressionOptions.AllowNullParameter);
expr.EvaluateParameter += value;
res = expr.Evaluate();
Console.WriteLine(res); // 输出: <每公斤价格> 1 Litro/ 1 Kg 3.90 Euros

expr = new Expression("Weight()", ExpressionOptions.StringConcat | ExpressionOptions.AllowNullParameter);
expr.EvaluateFunction += Expr_EvaluateFunction;


res = expr.Evaluate();
Console.WriteLine(res); // 输出: <每公斤价格> 1 Litro/ 1 Kg 3.90 Euros
