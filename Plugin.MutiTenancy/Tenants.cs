using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.MultiTenancy
{
	//appsettings.json node:
	//"Tenants":{
	//	"Items":[
	//		{
	//			"Name":"t1",
	//			"RequestUrlPrefix":"t1"
	//		},
	//		{
	//			"Name":"t2",
	//			"RequestUrlPrefix":"t2"
	//		}
	//	]
	//},
    public class Tenants
    {
        public List<Tenant> Items { get; set; }
    }

    public class Tenant
    {
        public string Name { get; set; }
        public string RequestUrlPrefix { get; set; }
    }
}
