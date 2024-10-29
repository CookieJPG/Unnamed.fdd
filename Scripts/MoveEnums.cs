using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum xAxis
{
    [Description("Horizontal1")]
    Player1 = 1,
    [Description("Horizontal2")]
    Player2 = 2,
    [Description("Horizontal3")]
    Player3 = 3,
    [Description("Horizontal4")]
    Player4 = 4,
}
public enum yAxis
{
    [Description("Vertical1")]
    Player1 = 1,
    [Description("Vertical2")]
    Player2 = 2,
    [Description("Vertical3")]
    Player3 = 3,
    [Description("Vertical4")]
    Player4 = 4,
}
public enum DashInput
{
    [Description("Dash1")]
    Player1 = 1,
    [Description("Dash2")]
    Player2 = 2,
    [Description("Dash3")]
    Player3 = 3,
    [Description("Dash4")]
    Player4 = 4,
}
public enum JumpInput
{
    [Description("Jump1")]
    Player1 = 1,
    [Description("Jump2")]
    Player2 = 2,
    [Description("Jump3")]
    Player3 = 3,
    [Description("Jump4")]
    Player4 = 4,
}
public enum AttackInput
{
    [Description("Attack1")]
    Player1 = 1,
    [Description("Attack2")]
    Player2 = 2,
    [Description("Attack3")]
    Player3 = 3,
    [Description("Attack4")]
    Player4 = 4,
}
public static class EnToStr
{
    public static string EnumString(this xAxis val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
           .GetType()
           .GetField(val.ToString())
           .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
    public static string EnumString(this yAxis val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
           .GetType()
           .GetField(val.ToString())
           .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
    public static string EnumString(this JumpInput val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
           .GetType()
           .GetField(val.ToString())
           .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
    public static string EnumString(this DashInput val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
           .GetType()
           .GetField(val.ToString())
           .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
    public static string EnumString(this AttackInput val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
           .GetType()
           .GetField(val.ToString())
           .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
}
