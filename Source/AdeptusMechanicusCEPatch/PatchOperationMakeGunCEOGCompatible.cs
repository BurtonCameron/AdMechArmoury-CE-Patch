using System;
using System.Xml;
using Verse;

namespace CombatExtended
{
	// Token: 0x0200003A RID: 58
	public class PatchOperationMakeGunCEOGCompatible : PatchOperation
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x0000AA98 File Offset: 0x00008C98
		protected override bool ApplyWorker(XmlDocument xml)
		{
			bool flag = false;
			bool flag2 = GenText.NullOrEmpty(this.defName);
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				foreach (object obj in xml.SelectNodes("Defs/ThingDef[defName=\"" + this.defName + "\"]"))
				{
					flag = true;
					XmlNode xmlNode = obj as XmlNode;
					XmlContainer xmlContainer = this.statBases;
					bool flag3 = xmlContainer != null && xmlContainer.node.HasChildNodes;
					if (flag3)
					{
						this.AddOrReplaceStatBases(xml, xmlNode);
					}
					XmlContainer xmlContainer2 = this.costList;
					bool flag4 = xmlContainer2 != null && xmlContainer2.node.HasChildNodes;
					if (flag4)
					{
						this.AddOrReplaceCostList(xml, xmlNode);
					}
					bool flag5 = this.Properties != null && this.Properties.node.HasChildNodes;
					if (flag5)
					{
						this.AddOrReplaceVerbPropertiesCE(xml, xmlNode);
					}
					bool flag6 = this.AmmoUser != null || this.FireModes != null;
					if (flag6)
					{
						this.AddOrReplaceCompsCE(xml, xmlNode);
					}
					bool flag7 = this.weaponTags != null && this.weaponTags.node.HasChildNodes;
					if (flag7)
					{
						this.AddOrReplaceWeaponTags(xml, xmlNode);
					}
					bool flag8 = this.researchPrerequisite != null;
					if (flag8)
					{
						this.AddOrReplaceResearchPrereq(xml, xmlNode);
					}
					bool flag9 = ModLister.HasActiveModWithName("RunAndGun") && !this.AllowWithRunAndGun;
					if (flag9)
					{
						this.AddRunAndGunExtension(xml, xmlNode);
					}
				}
				result = flag;
			}
			return result;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000AC58 File Offset: 0x00008E58
		private bool GetOrCreateNode(XmlDocument xml, XmlNode xmlNode, string name, out XmlElement output)
		{
			XmlNodeList xmlNodeList = xmlNode.SelectNodes(name);
			bool flag = xmlNodeList.Count == 0;
			bool result;
			if (flag)
			{
				output = xml.CreateElement(name);
				xmlNode.AppendChild(output);
				result = false;
			}
			else
			{
				output = (xmlNodeList[0] as XmlElement);
				result = true;
			}
			return result;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000ACAC File Offset: 0x00008EAC
		private XmlElement CreateListElementAndPopulate(XmlDocument xml, XmlNode reference, string type = null)
		{
			XmlElement xmlElement = xml.CreateElement("li");
			bool flag = type != null;
			if (flag)
			{
				xmlElement.SetAttribute("Class", type);
			}
			this.Populate(xml, reference, ref xmlElement, false);
			return xmlElement;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000ACF0 File Offset: 0x00008EF0
		private void Populate(XmlDocument xml, XmlNode reference, ref XmlElement destination, bool overrideExisting = false)
		{
			foreach (object obj in reference)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (overrideExisting)
				{
					XmlNodeList xmlNodeList = destination.SelectNodes(xmlNode.Name);
					bool flag = xmlNodeList != null;
					if (flag)
					{
						foreach (object obj2 in xmlNodeList)
						{
							XmlNode oldChild = (XmlNode)obj2;
							destination.RemoveChild(oldChild);
						}
					}
				}
				destination.AppendChild(xml.ImportNode(xmlNode, true));
			}
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000ADD0 File Offset: 0x00008FD0
		private void AddOrReplaceVerbPropertiesCE(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			bool orCreateNode = this.GetOrCreateNode(xml, xmlNode, "verbs", out xmlElement);
			if (orCreateNode)
			{
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("li[verbClass=\"Verb_Shoot\" or verbClass=\"Verb_ShootOneUse\" or verbClass=\"Verb_LaunchProjectile\"]");
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode2 = obj as XmlNode;
					bool flag = xmlNode2 != null;
					if (flag)
					{
						xmlElement.RemoveChild(xmlNode2);
					}
				}
			}
			xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, this.Properties.node, "CombatExtended.VerbPropertiesCEOG"));
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000AE80 File Offset: 0x00009080
		private void AddOrReplaceCompsCE(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "comps", out xmlElement);
			bool flag = this.AmmoUser != null;
			if (flag)
			{
				xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, this.AmmoUser.node, "CombatExtended.CompProperties_AmmoUser"));
			}
			bool flag2 = this.FireModes != null;
			if (flag2)
			{
				xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, this.FireModes.node, "CombatExtended.CompProperties_FireModes"));
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000AEF8 File Offset: 0x000090F8
		private void AddOrReplaceWeaponTags(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "weaponTags", out xmlElement);
			this.Populate(xml, this.weaponTags.node, ref xmlElement, false);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000AF2C File Offset: 0x0000912C
		private void AddOrReplaceStatBases(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "statBases", out xmlElement);
			bool hasChildNodes = xmlElement.HasChildNodes;
			if (hasChildNodes)
			{
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("AccuracyTouch | AccuracyShort | AccuracyMedium | AccuracyLong");
				foreach (object obj in xmlNodeList)
				{
					XmlNode oldChild = (XmlNode)obj;
					xmlElement.RemoveChild(oldChild);
				}
			}
			this.Populate(xml, this.statBases.node, ref xmlElement, true);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000AFCC File Offset: 0x000091CC
		private void AddOrReplaceCostList(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "costList", out xmlElement);
			bool hasChildNodes = xmlElement.HasChildNodes;
			if (hasChildNodes)
			{
				xmlElement.RemoveAll();
			}
			this.Populate(xml, this.costList.node, ref xmlElement, false);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000B014 File Offset: 0x00009214
		private void AddOrReplaceResearchPrereq(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "recipeMaker", out xmlElement);
			XmlNode xmlNode2 = xmlElement.SelectSingleNode(this.researchPrerequisite.node.Name);
			bool flag = xmlNode2 != null;
			if (flag)
			{
				xmlElement.ReplaceChild(xml.ImportNode(this.researchPrerequisite.node, true), xmlNode2);
			}
			else
			{
				xmlElement.AppendChild(xml.ImportNode(this.researchPrerequisite.node, true));
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000B08C File Offset: 0x0000928C
		private void AddRunAndGunExtension(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "modExtensions", out xmlElement);
			XmlElement xmlElement2 = xml.CreateElement("li");
			xmlElement2.SetAttribute("Class", "RunAndGun.DefModExtension_SettingDefaults");
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = xml.CreateElement("weaponForbidden");
			xmlElement3.InnerText = "true";
			xmlElement2.AppendChild(xmlElement3);
		}

		// Token: 0x040000A7 RID: 167
		public string defName;

		// Token: 0x040000A8 RID: 168
		public bool AllowWithRunAndGun = true;

		// Token: 0x040000A9 RID: 169
		public XmlContainer statBases;

		// Token: 0x040000AA RID: 170
		public XmlContainer Properties;

		// Token: 0x040000AB RID: 171
		public XmlContainer AmmoUser;

		// Token: 0x040000AC RID: 172
		public XmlContainer FireModes;

		// Token: 0x040000AD RID: 173
		public XmlContainer weaponTags;

		// Token: 0x040000AE RID: 174
		public XmlContainer costList;

		// Token: 0x040000AF RID: 175
		public XmlContainer researchPrerequisite;
	}
}
