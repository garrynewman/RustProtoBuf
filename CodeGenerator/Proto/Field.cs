using System;

namespace SilentOrbit.ProtocolBuffers
{
    class Field : IComment
    {
        public readonly SourcePath Source;

        public Field(TokenReader tr)
        {
            Source = new SourcePath(tr);
        }
        #region .proto data
        /// <summary>
        /// Comments written before the field in the .proto file.
        /// These comments will be written into the generated code.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// required/optional/repeated as read from .proto file
        /// </summary>
        public FieldRule Rule { get; set; }

        /// <summary>
        /// Field type as read from the .proto file
        /// </summary>
        public string ProtoTypeName { get; set; }

        /// <summary>
        /// Field name read from the .proto file
        /// </summary>
        public string ProtoName { get; set; }

        /// <summary>
        /// Field name in generated c# code.
        /// </summary>
        public string CsName { get; set; }

        /// <summary>
        /// Wire format ID
        /// </summary>
        public int ID { get; set; }
        //Field options
        public bool OptionPacked = false;
        public bool OptionDeprecated = false;
		public bool OptionUseReferences = true;
        public string OptionDefault = null;
        public bool OptionPooled = false;
        public bool OptionUidClear = false;

        public bool IsUsingBinaryWriter
        {
            get
            {
                if (ProtoType.WireType == Wire.Fixed32)
                    return true;
                if (ProtoType.WireType == Wire.Fixed64)
                    return true;
                return false;
            }
        }
        #region Locally used fields
        //These options are not the build in ones and have a meaning in the code generation
        /// <summary>
        /// Define the access of the field: public, protected, private or internal
        /// </summary>
        public string OptionAccess = "public";

        /// <summary>
        /// <para>Define the type of the property that is not a primitive or class derived from a message.</para>
        /// <para>This can be one of the build in (see method MessageCode.GenerateFieldTypeWriter()) or a custom class that implements the static Serialize and Deserialize functions;</para>
        /// </summary>
        public string OptionCodeType = null;

        /// <summary>
        /// Property is written elsewhere, in another file using partial, code will not be generated for this field
        /// </summary>
        public bool OptionExternal = false;

        /// <summary>
        /// Field is (c#)readonly.
        /// Can be set to true if OptionGenerate=false and your own code 
        /// </summary>
        public bool OptionReadOnly = false;

        /// <summary>
        /// Initial capacity of allocated MemoryStream when Serializing this object.
        /// Size in bytes.
        /// </summary>
        public int BufferSize { get; set; }
        #endregion
        #endregion

        public Wire WireType
        {
            get
            {
                if (OptionPacked)
                    return Wire.LengthDelimited;
                return ProtoType.WireType;
            }
        }
        #region Code Generation Properties
        //These are generated as a second stage parsing of the .proto file.
        //They are used in the code generation.
        /// <summary>
        /// .proto type including enum and message.
        /// </summary>
        public ProtoType ProtoType { get; set; }
        #endregion
        public override string ToString()
        {
            return string.Format("{0} {1} {2} = {3}", Rule, ProtoTypeName, ProtoName, ID);
        }
    }
}

