//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Notestash_Admin
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblTemplate
    {
        public int Id { get; set; }
        public string Template { get; set; }
        public int NoteId { get; set; }
    
        public virtual tblUser tblUser { get; set; }
    }
}
