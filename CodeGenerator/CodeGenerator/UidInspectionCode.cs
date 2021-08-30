using System;
using SilentOrbit.Code;

namespace SilentOrbit.ProtocolBuffers
{
    static class InspectionCode
    {
        public static void GenerateShared(CodeWriter cw, Options options)
        {
            cw.Bracket("public enum UidType");
            cw.WriteLine("Entity,");
            cw.WriteLine("Item,");
            cw.EndBracket();
            cw.WriteLine();
            cw.WriteLine("public delegate void UidInspector<T>(UidType type, ref T value);");
            cw.WriteLine();
        }

        public static void GenerateUidInspector(ProtoMessage m, CodeWriter cw, Options options)
        {
            cw.Bracket("public void InspectUids(UidInspector<uint> action)");

            foreach (var f in m.Fields.Values)
            {
                if (!string.IsNullOrWhiteSpace(f.OptionUid))
                {
                    if (f.ProtoType.ProtoName != ProtoBuiltin.UInt32)
                    {
                        throw new Exception($"{m.FullCsType}::{f.CsName} is tagged as a uid but is not a uint32");
                    }

                    // TODO: support repeated
                    cw.WriteLine($"action(UidType.{f.OptionUid}, ref {f.CsName});");
                }
                else if (f.ProtoType is ProtoMessage message && !message.OptionExternal)
                {
                    var isRepeated = f.Rule == FieldRule.Repeated;
                    var itemName = isRepeated ? "_item" : f.CsName;

                    if (isRepeated)
                    {
                        cw.IfBracket($"{f.CsName} != null");
                        cw.ForeachBracket("_item", f.CsName);
                    }

                    if (f.ProtoType.Nullable)
                    {
                        cw.WriteLine($"{itemName}?.InspectUids(action);");
                    }
                    else
                    {
                        cw.WriteLine($"{itemName}.InspectUids(action);");
                    }

                    if (isRepeated)
                    {
                        cw.EndBracket();
                        cw.EndBracket();
                    }
                }
            }

            cw.EndBracket();
        }
    }
}
