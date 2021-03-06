﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tavisca.Frameworks.Session.Provider.SQL
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="dScepterDB")]
	public partial class DataStoreDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public DataStoreDataContext() : 
				base(global::Tavisca.Frameworks.Session.Properties.Settings.Default.dScepterDBConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DataStoreDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataStoreDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataStoreDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataStoreDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.spAddEntry")]
		public int spAddEntry([global::System.Data.Linq.Mapping.ParameterAttribute(Name="Category", DbType="NVarChar(64)")] string category, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="Key", DbType="UniqueIdentifier")] System.Nullable<System.Guid> key, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="Value", DbType="VarBinary(MAX)")] System.Data.Linq.Binary value, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="AddedOnUtc", DbType="DateTime")] System.Nullable<System.DateTime> addedOnUtc, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="ExpiresOnUtc", DbType="DateTime")] System.Nullable<System.DateTime> expiresOnUtc)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), category, key, value, addedOnUtc, expiresOnUtc);
			return ((int)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.spRemoveEntry")]
		public int spRemoveEntry([global::System.Data.Linq.Mapping.ParameterAttribute(Name="Key", DbType="UniqueIdentifier")] System.Nullable<System.Guid> key, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="Category", DbType="NVarChar(64)")] string category)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), key, category);
			return ((int)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.spGetAllEntriesForKey")]
		public ISingleResult<spGetAllEntriesForKeyResult> spGetAllEntriesForKey([global::System.Data.Linq.Mapping.ParameterAttribute(Name="Key", DbType="UniqueIdentifier")] System.Nullable<System.Guid> key)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), key);
			return ((ISingleResult<spGetAllEntriesForKeyResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.spGetEntry")]
		public ISingleResult<spGetEntryResult> spGetEntry([global::System.Data.Linq.Mapping.ParameterAttribute(Name="Key", DbType="UniqueIdentifier")] System.Nullable<System.Guid> key, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="Category", DbType="NVarChar(64)")] string category)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), key, category);
			return ((ISingleResult<spGetEntryResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.spRemoveAllEntriesForKey")]
		public int spRemoveAllEntriesForKey([global::System.Data.Linq.Mapping.ParameterAttribute(Name="Key", DbType="UniqueIdentifier")] System.Nullable<System.Guid> key)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), key);
			return ((int)(result.ReturnValue));
		}
	}
	
	public partial class spGetAllEntriesForKeyResult
	{
		
		private System.Guid _ObjectKey;
		
		private string _Category;
		
		private System.Data.Linq.Binary _ObjectValue;
		
		private System.DateTime _AddedOnUTC;
		
		private System.DateTime _ExpiresOnUTC;
		
		public spGetAllEntriesForKeyResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectKey", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid ObjectKey
		{
			get
			{
				return this._ObjectKey;
			}
			set
			{
				if ((this._ObjectKey != value))
				{
					this._ObjectKey = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Category", DbType="NVarChar(64) NOT NULL", CanBeNull=false)]
		public string Category
		{
			get
			{
				return this._Category;
			}
			set
			{
				if ((this._Category != value))
				{
					this._Category = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectValue", DbType="VarBinary(MAX)")]
		public System.Data.Linq.Binary ObjectValue
		{
			get
			{
				return this._ObjectValue;
			}
			set
			{
				if ((this._ObjectValue != value))
				{
					this._ObjectValue = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddedOnUTC", DbType="DateTime NOT NULL")]
		public System.DateTime AddedOnUTC
		{
			get
			{
				return this._AddedOnUTC;
			}
			set
			{
				if ((this._AddedOnUTC != value))
				{
					this._AddedOnUTC = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ExpiresOnUTC", DbType="DateTime NOT NULL")]
		public System.DateTime ExpiresOnUTC
		{
			get
			{
				return this._ExpiresOnUTC;
			}
			set
			{
				if ((this._ExpiresOnUTC != value))
				{
					this._ExpiresOnUTC = value;
				}
			}
		}
	}
	
	public partial class spGetEntryResult
	{
		
		private System.Guid _ObjectKey;
		
		private string _Category;
		
		private System.Data.Linq.Binary _ObjectValue;
		
		private System.DateTime _AddedOnUTC;
		
		private System.DateTime _ExpiresOnUTC;
		
		public spGetEntryResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectKey", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid ObjectKey
		{
			get
			{
				return this._ObjectKey;
			}
			set
			{
				if ((this._ObjectKey != value))
				{
					this._ObjectKey = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Category", DbType="NVarChar(64) NOT NULL", CanBeNull=false)]
		public string Category
		{
			get
			{
				return this._Category;
			}
			set
			{
				if ((this._Category != value))
				{
					this._Category = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectValue", DbType="VarBinary(MAX)")]
		public System.Data.Linq.Binary ObjectValue
		{
			get
			{
				return this._ObjectValue;
			}
			set
			{
				if ((this._ObjectValue != value))
				{
					this._ObjectValue = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddedOnUTC", DbType="DateTime NOT NULL")]
		public System.DateTime AddedOnUTC
		{
			get
			{
				return this._AddedOnUTC;
			}
			set
			{
				if ((this._AddedOnUTC != value))
				{
					this._AddedOnUTC = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ExpiresOnUTC", DbType="DateTime NOT NULL")]
		public System.DateTime ExpiresOnUTC
		{
			get
			{
				return this._ExpiresOnUTC;
			}
			set
			{
				if ((this._ExpiresOnUTC != value))
				{
					this._ExpiresOnUTC = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
