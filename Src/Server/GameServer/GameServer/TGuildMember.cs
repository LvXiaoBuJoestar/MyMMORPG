//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace GameServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class TGuildMember
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public int Class { get; set; }
        public int Level { get; set; }
        public int Title { get; set; }
        public System.DateTime JoinTime { get; set; }
        public System.DateTime LastTime { get; set; }
        public int TGuildId { get; set; }
    
        public virtual TGuild Guild { get; set; }
    }
}
