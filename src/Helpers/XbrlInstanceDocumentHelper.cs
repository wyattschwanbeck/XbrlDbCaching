using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
namespace Xbrl.Helpers
{
    public static class XbrlInstanceDocumentHelper
    {
        public static XbrlFact GetValueFromPriorities(this XbrlInstanceDocument doc, string context_id, params string[] priorities)
        {
            //First get the normal period context
            string normal_period_context = context_id;
            
            foreach (string word in priorities)
            {//[@contextRef=\"{1}\"]"
                XmlNodeList detail = doc.xmlDocument.SelectNodes(string.Format("//*[local-name()='{0}' and @contextRef=\"{1}\"]", word, normal_period_context, word.Length), doc.xmlNamespaceManager);
                
                if (detail.Count > 0)
                {
                    return detail[0].ToFact(doc.xmlNamespaceManager);
                } else
                {
                    XmlNode result = doc.xmlDocument.SelectSingleNode(String.Format("//*[local-name()='{0}' and @contextRef='{1}']", word, doc.PrimaryInstantContextId),doc.xmlNamespaceManager);
                    if(result!=null)
                        return result.ToFact(doc.xmlNamespaceManager);
                    //return null;
                }
            }
            


            throw new Exception("Unable to find XBRL fact for value labeled with any of the following: " + priorities);
        }
        public static XbrlFact ToFact(this XmlNode node, XmlNamespaceManager nsmgr)
        {
            //Context isn't needed again and facts should have at least two attributes for fact ID and context
            if (node.Name != "context" && node.Attributes.Count > 0)
            {
                XbrlFact fact = new XbrlFact();
                fact.Value = node.InnerText;
                fact.NamespaceId = nsmgr.LookupPrefix(node.NamespaceURI);
                fact.Label = node.Name.Contains(":") ? node.Name.Split(':')[1] : node.Name;


                foreach (XmlAttribute attr in node.Attributes)
                {
                    //Populate fact attributes
                    if (attr.Name == "contextRef")
                    {
                        fact.ContextId = attr.Value;

                    }
                    //Decimals can be INF sometimes, those are defaulted to null
                    else if (attr.Name == "decimals")
                    {
                        int factDec;
                        int.TryParse(attr.Value, out factDec);
                        if (int.TryParse(attr.Value, out factDec))
                        {
                            fact.Decimals = factDec;
                        }
                    }
                    else if (attr.Name == "id")
                    {
                        fact.Id = attr.Value;
                    }
                    else if (attr.Name == "unitRef")
                    {
                        fact.UnitId = attr.Value;
                    }
                }
                return fact;
            }
            else return null;
        }
        //public static XbrlFact GetValueFromPriorities(this XbrlInstanceDocument doc, string context_id, params string[] priorities)
        //{
        //    //First get the normal period context
        //    string normal_period_context = context_id;

        //    foreach (string word in priorities)
        //    {
        //        foreach (XbrlFact fact in doc.Facts)
        //        {
        //            if (fact.ContextId.ToLower().Trim() == doc.PrimaryInstantContextId.ToLower().Trim() || fact.ContextId.Trim().ToLower() == normal_period_context.Trim().ToLower())
        //            {
        //                if (fact.Label.Trim().ToLower() == word.Trim().ToLower())
        //                {
        //                    return fact;
        //                }
        //            }
        //        }
        //    }


        //    throw new Exception("Unable to find XBRL fact for value labeled with any of the following: " + priorities);
        //}

        public static XbrlFact GetValueFromPriorities(this XbrlInstanceDocument doc, params string[] priorities)
        {
            //First get the normal period context
            string normal_period_context = doc.FindNormalPeriodPrimaryContext().Id;
            return doc.GetValueFromPriorities(normal_period_context, priorities);            
        }
    }
}