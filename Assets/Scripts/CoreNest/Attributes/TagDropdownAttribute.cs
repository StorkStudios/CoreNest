using System;
using UnityEngine;

/// <summary>
/// Attribute for string field that draws a dropdown for selecting a tag
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class TagDropdownAttribute : PropertyAttribute
{

}
