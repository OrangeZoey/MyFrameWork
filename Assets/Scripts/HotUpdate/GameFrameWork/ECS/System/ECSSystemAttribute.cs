
/// <summary>
/// 自定义特性   限制为类  可继承  一个类不能应用多个此特性
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class ECSSystemAttribute : System.Attribute
{
}

