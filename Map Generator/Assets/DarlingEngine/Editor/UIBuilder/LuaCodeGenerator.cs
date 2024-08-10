using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DarlingEngine.Engine.Bundle;
using System.IO;
using System;
using DarlingEngine.Engine;

namespace DarlingEngine.Editor.UIBuilder
{
    internal class LuaCodeTemplate
    {
        private readonly string luaTamplate;

        public LuaCodeTemplate(string templatePath)
        {
            var templateBytes = GameUtil.File2UTF8(templatePath) ?? throw new Exception($"LuaCodeTemplate: {templatePath} is not exist!");
            luaTamplate = GameUtil.Bytes2String(templateBytes);
        }

        public string Render(object values)
        {
            string output = luaTamplate;
            foreach (var p in values.GetType().GetProperties())
                output = output.Replace("[%" + p.Name + "%]", (p.GetValue(values, null) as string) ?? string.Empty);
            return output;
        }
    }

    internal static class LuaCodeGenerator
    {
        private const string luaCodeTemplatePath = PathProtocol.LuaProjectDir + "Template/UI_lua.txt";
        private const string metaLuaCodeTemplatePath = PathProtocol.LuaProjectDir + "Template/UI_Meta_lua.txt";
        private const string modelLuaCodeTemplatePath = PathProtocol.LuaProjectDir + "Template/UI_Model_lua.txt";

        private static string RenderTemplate(string templateFilePath, object values)
        {
            var template = new LuaCodeTemplate(templateFilePath);
            return template.Render(values);
        }

        public static void GenerateLuaScript(string prefabPath)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            string luaTargetDir = Path.Combine(PathProtocol.LuaSourceCodeWindowsDir, prefab.name);
            if (!Directory.Exists(luaTargetDir))
            {
                Directory.CreateDirectory(luaTargetDir);
            }
            string metaLuaCodePath = Path.Combine(luaTargetDir, prefab.name + "Meta.lua");
            string luaCodePath = Path.Combine(luaTargetDir, prefab.name + ".lua");
            string modelLuaCodePath = Path.Combine(luaTargetDir, prefab.name + "Model.lua");
            var foundDlComponents = new Dictionary<string, List<string>>();
            // 遍历Prefab中的所有子节点，找到所有的DLComponent
            Transform[] allTransformInPrefab = prefab.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allTransformInPrefab)
            {
                if (!foundDlComponents.TryGetValue(child.name, out var comps))
                {
                    comps = new List<string>();
                }
                foreach (var typeName in Enum.GetNames(typeof(DLComponentType)))
                {
                    if (child.GetComponent(typeName))
                    {
                        comps.Add(typeName);
                    }
                }
                foundDlComponents[child.name] = comps;
            }

            string metaOnInit = null;
            string metaOnDispose = null;
            string metaOnRegister = null;
            string metaOnRemove = null;
            string metaAppendCode = null;

            foreach (var kv in foundDlComponents)
            {
                var nodeName = kv.Key;
                var comps = kv.Value;
                foreach (var c in comps)
                {
                    var typeName = (DLComponentType)Enum.Parse(typeof(DLComponentType), c);
                    metaOnInit += MetaOnInit(nodeName, typeName);
                    metaOnDispose += MetaOnDispose(nodeName, typeName);
                    metaOnRegister += MetaOnRegister(nodeName, typeName);
                    metaOnRemove += MetaOnRemove(nodeName, typeName);
                    metaAppendCode += MetaAppendCode(nodeName, typeName, prefab.name);
                }
            }

            string luaCode = RenderTemplate(luaCodeTemplatePath, new
            {
                PREFAB_NAME = prefab.name,
            });
            if (!File.Exists(luaCodePath))
            {
                File.WriteAllText(luaCodePath, luaCode);
                Debug.LogFormat($"file {luaCodePath} is generated!");
            }
            else
            {
                Debug.LogWarning($"file {luaCodePath} already exist, abort to generate this file");
            }
            string modelLuaCode = RenderTemplate(modelLuaCodeTemplatePath, new
            {
                PREFAB_NAME = prefab.name,
            });
            if (!File.Exists(modelLuaCodePath))
            {
                File.WriteAllText(modelLuaCodePath, modelLuaCode);
                Debug.LogFormat($"file {modelLuaCodePath} is generated!");
            }
            else
            {
                Debug.LogWarning($"file {modelLuaCodePath} already exist, abort to generate this file");
            }
            string metaLuaCode = RenderTemplate(metaLuaCodeTemplatePath, new
            {
                PREFAB_NAME = prefab.name,
                META_ON_INIT = metaOnInit,
                META_ON_DISPOSE = metaOnDispose,
                META_ON_REGISTER = metaOnRegister,
                META_ON_REMOVE = metaOnRemove,
                META_APPEND_CODE = metaAppendCode,
            });
            if (File.Exists(metaLuaCodePath))
            {
                Debug.LogWarning($"file {metaLuaCodePath} already exist, will cover this file");
            }
            File.WriteAllText(metaLuaCodePath, metaLuaCode);
            Debug.LogFormat($"file {metaLuaCodePath} is generated!");

            AssetDatabase.Refresh();
        }

        private static string FullName(DLComponentType typeName)
        {
            if (typeName == DLComponentType.DLTransform)
            {
                return "DarlingEngine.Engine.DLTransform";
            }
            return "DarlingEngine.UI." + typeName;
        }

        public static bool IsValueChangedDLType(DLComponentType type)
        {
            switch (type)
            {
                case DLComponentType.DLInputField:
                case DLComponentType.DLSlider:
                    return true;
                case DLComponentType.DLToggle:
                case DLComponentType.DLScrollBar:
                case DLComponentType.DLScrollRect:
                case DLComponentType.DLDropdown:
                default:
                    return false;
            };
        }

        private static string MetaOnInit(string nodeName, DLComponentType type)
        {
            string luaName = $"{type}_{nodeName}";
            string fullName = FullName(type);
            return $"    ---@type {fullName}\n" +
                   $"    self.{luaName} = GameUtil.FindChild(self.Node, '{nodeName}', typeof({fullName}));\n";
        }

        private static string MetaOnDispose(string nodeName, DLComponentType type)
        {
            string luaName = $"{type}_{nodeName}";
            return $"    self.{luaName}:Dispose();\n" +
                   $"    self.{luaName} = nil;\n";
        }

        private static string MetaOnRegister(string nodeName, DLComponentType type)
        {
            string luaName = $"{type}_{nodeName}";
            if (type == DLComponentType.DLButton)
            {
                return $"    UnityEventHelper.RegisterEvent(self.{luaName}, UnityEventHelper.Click, function (data) self:On_{luaName}_Click(data) end);\n";
            }
            else if (IsValueChangedDLType(type))
            {
                return $"    UnityEventHelper.RegisterEvent(self.{luaName}, UnityEventHelper.ValueChanged, function (data) self:On_{luaName}_ValueChanged(data) end);\n" +
                       $"    UnityEventHelper.RegisterEvent(self.{luaName}, UnityEventHelper.EndEdit, function (data) self:On_{luaName}_EndEdit(data) end);\n" +
                       $"    UnityEventHelper.RegisterEvent(self.{luaName}, UnityEventHelper.Submit, function (data) self:On_{luaName}_Submit(data) end);\n";
            }
            else if (type == DLComponentType.DLButtonGroup)
            {
                return $"    UnityEventHelper.RegisterEvent(self.{luaName}, UnityEventHelper.Selected, function (data) self:On_{luaName}_Selected(data) end);\n";
            }
            else if (type == DLComponentType.DLDragButton)
            {
                return $"    UnityEventHelper.RegisterEvent(self.{luaName}, UnityEventHelper.Drag, function (data) self:On_{luaName}_Drag(data) end);\n" +
                       $"    UnityEventHelper.RegisterEvent(self.{luaName}, UnityEventHelper.Down, function (data) self:On_{luaName}_Down(data) end);\n" +
                       $"    UnityEventHelper.RegisterEvent(self.{luaName}, UnityEventHelper.Up, function (data) self:On_{luaName}_Up(data) end);\n";
            }
            return string.Empty;
        }

        private static string MetaOnRemove(string nodeName, DLComponentType type)
        {
            string luaName = $"{type}_{nodeName}";
            if (type == DLComponentType.DLButton)
            {
                return $"    UnityEventHelper.RemoveEvent(self.{luaName}, UnityEventHelper.Click);\n";
            }
            else if (IsValueChangedDLType(type))
            {
                return $"    UnityEventHelper.RemoveEvent(self.{luaName}, UnityEventHelper.ValueChanged);\n" +
                       $"    UnityEventHelper.RemoveEvent(self.{luaName}, UnityEventHelper.EndEdit);\n" +
                       $"    UnityEventHelper.RemoveEvent(self.{luaName}, UnityEventHelper.Submit);\n";
            }
            else if (type == DLComponentType.DLButtonGroup)
            {
                return $"    UnityEventHelper.RemoveEvent(self.{luaName}, UnityEventHelper.Selected);\n";
            }
            else if (type == DLComponentType.DLDragButton)
            {
                return $"    UnityEventHelper.RemoveEvent(self.{luaName}, UnityEventHelper.Drag);\n" +
                       $"    UnityEventHelper.RemoveEvent(self.{luaName}, UnityEventHelper.Down);\n" +
                       $"    UnityEventHelper.RemoveEvent(self.{luaName}, UnityEventHelper.Up);\n";
            }
            return string.Empty;
        }

        private static string MetaAppendCode(string nodeName, DLComponentType typeName, string prefabName)
        {
            string luaName = $"{typeName}_{nodeName}";
            if (typeName == DLComponentType.DLButton)
            {
                return $"---事件函数---\n" +
                       $"function {prefabName}Meta:On_{luaName}_Click()\n" +
                       $"end\n";
            }
            else if (IsValueChangedDLType(typeName))
            {
                return $"---事件函数---\n" +
                       $"---@param data System.Object[]\n" +
                       $"function {prefabName}Meta:On_{luaName}_ValueChanged(data)\n" +
                       $"end\n" +
                       $"\n" +
                       $"---事件函数---\n" +
                       $"---@param data System.Object[]\n" +
                       $"function {prefabName}Meta:On_{luaName}_EndEdit(data)\n" +
                       $"end\n" +
                       $"\n" +
                       $"---事件函数---\n" +
                       $"---@param data System.Object[]\n" +
                       $"function {prefabName}Meta:On_{luaName}_Submit(data)\n" +
                       $"end\n" +
                       $"\n";
            }
            else if (typeName == DLComponentType.DLButtonGroup)
            {
                return $"---事件函数---\n" +
                       $"---@param data System.Object[]\n" +
                       $"function {prefabName}Meta:On_{luaName}_Selected(data)\n" +
                       $"    ---@type number\n" +
                       $"    local selected = data[0]\n" +
                       $"end\n";
            }
            else if (typeName == DLComponentType.DLDragButton)
            {
                return $"---事件函数---\n" +
                       $"---@param data System.Object[]\n" +
                       $"function {prefabName}Meta:On_{luaName}_Drag(data)\n" +
                       $"    ---@type UnityEngine.EventSystems.PointerEventData\n" +
                       $"    local arg = data[0]\n" +
                       $"end\n" +
                       $"\n" +
                       $"---事件函数---\n" +
                       $"---@param data System.Object[]\n" +
                       $"function {prefabName}Meta:On_{luaName}_Down(data)\n" +
                       $"    ---@type UnityEngine.EventSystems.PointerEventData\n" +
                       $"    local arg = data[0]\n" +
                       $"end\n" +
                       $"\n" +
                       $"---事件函数---\n" +
                       $"---@param data System.Object[]\n" +
                       $"function {prefabName}Meta:On_{luaName}_Up(data)\n" +
                       $"    ---@type UnityEngine.EventSystems.PointerEventData\n" +
                       $"    local arg = data[0]\n" +
                       $"end\n" +
                       $"\n";
            }
            return "";
        }
    }
}