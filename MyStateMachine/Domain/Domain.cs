using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyStateMachine.Domain
{
	#region SampleWizard
	public class Individual
	{
		public string CUSTOMER_NO { get; set; }
		public string SURNAME_TX { get; set; }
		public string FIRST_NAME_TX { get; set; }
		public string INITIALS_TX { get; set; }
		public string OCCUPATION_CD { get; set; }
		public string MARITL_TYP_CD { get; set; }
		public string TITL_TYP_CD { get; set; }
		public string EMP_STA_CD { get; set; }
		public string GENDER_CD { get; set; }
		public string LIFE_ST_CD { get; set; }
		public string NAME_CHG_EVID_IN { get; set; }
		public DateTime UPDATE_DTS { get; set; }
		public string UPDATE_USER_ID { get; set; }
		public string MKT_LTR_SPRS_IN { get; set; }
	}
	public class Preference
	{
		public string FileName { get; set; }
	}
	public class Database
	{
		public string DatabaseName { get; set; }
		public string TableName { get; set; }
	}
	public class Override
	{
		public string OverrideName { get; set; }
		public bool IncludeGenericViewModel { get; set; }
	}
	#endregion

	#region DefenceCondition
	public class defcon1
	{
		public string Name { get; set; }
		public string EventName { get; set; }
	}
	public class defcon2
	{
		public string Name { get; set; }
		public string EventName { get; set; }
	}
	public class defcon3
	{
		public string Name { get; set; }
		public string EventName { get; set; }
	}
	#endregion
	#region OnlineShopping
	public class Shopping
	{
		public string Name { get; set; }
		public string Category { get; set; }
	}
	public class Searching
	{
		public string Name { get; set; }
		public string Category { get; set; }
		public string Product { get; set; }
		public string Supplier { get; set; }
	}
	public class Browsing
	{
		public string Name { get; set; }
		public string Category { get; set; }
		public string Product { get; set; }
		public string Supplier { get; set; }
	}
	public class SearchResult
	{
		public string Name { get; set; }
		public string Category { get; set; }
		public string Product { get; set; }
		public string Supplier { get; set; }
	}
	public class ViewingItem
	{
		public string Name { get; set; }

		public string Category { get; set; }
		public string Product { get; set; }
		public string Supplier { get; set; }
		public decimal UnitPrice { get; set; }
	}
	public class AddingToCart
	{
		public string Name { get; set; }
		public string EventName { get; set; }
	}
	public class ViewingCart
	{
		public string Name { get; set; }
		public string EventName { get; set; }
	}
	public class UpdatingCart
	{
		public string Name { get; set; }
		public string EventName { get; set; }
	}
	public class CheckingOut
	{
		public string Name { get; set; }
		public string EventName { get; set; }
	}
	# endregion

	#region Questionnaire
	#endregion
}
