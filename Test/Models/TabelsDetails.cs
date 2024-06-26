﻿using System.ComponentModel;

namespace Test.Models
{
    public class TabelsDetails
    {
        [DisplayName("Column Name")]
        public string COLUMN_NAME { get; set; }
        
        [DisplayName("Is Nullable")]
        public bool IS_NULLABLE { get; set; }

        [DisplayName("Data Type")]
        public string DATA_TYPE { get; set; }

        [DisplayName("Is PrimaryKey")]
        public bool isPrimaryKey { get; set; }

        [DisplayName("Select")]
        public bool isSelect { get; set; }

        [DisplayName("Update")]
        public bool isUpdate { get; set; }

        [DisplayName("Delete")]
        public bool isDelete { get; set; }
    }
}