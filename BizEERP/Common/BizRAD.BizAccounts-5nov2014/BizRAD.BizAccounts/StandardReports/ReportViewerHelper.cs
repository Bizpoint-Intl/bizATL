using System;
using System.IO;
using System.Drawing.Printing;
using System.Xml;

namespace BizRAD.BizAccounts
{
    class ReportViewerHelper
    {
        private XmlDocument oXmlDoc;

        const double dCM2Inches = 2.54000000259d;

        private double dPageWidth;
        private double dPageHeight;
        private double dMarginLeft;
        private double dMarginRight;
        private double dMarginTop;
        private double dMarginBottom;


        private double dPageWidthOld;
        private double dPageHeightOld;
        private double dMarginLeftOld;
        private double dMarginRightOld;
        private double dMarginTopOld;
        private double dMarginBottomOld;


        public ReportViewerHelper(string sXMLData)
        {
            oXmlDoc = new XmlDocument();

            oXmlDoc.LoadXml(sXMLData);
        }

        public void SetPageSize(PageSettings oSettings)
        {
            //Convert 100th of inch to cm
            dPageWidth = ((double)oSettings.Bounds.Width / 100) * dCM2Inches;
            dPageHeight = ((double)oSettings.Bounds.Height / 100) * dCM2Inches;

            dMarginLeft = ((double)oSettings.Margins.Left / 100) * dCM2Inches;
            dMarginRight = ((double)oSettings.Margins.Right / 100) * dCM2Inches;
            dMarginTop = ((double)oSettings.Margins.Top / 100) * dCM2Inches;
            dMarginBottom = ((double)oSettings.Margins.Bottom / 100) * dCM2Inches;

            XmlNode oRoot = oXmlDoc.DocumentElement;

            XmlNode oNode;

            //Save the current values for the PageWidht, PageHeight and Margins
            //Only supports values in cm
            oNode = GetNode("PageWidth", oRoot);
            dPageWidthOld = double.Parse(oNode.InnerText.Replace("cm", ""));
            oNode.InnerText = dPageWidth.ToString(".0") + "cm";

            oNode = GetNode("PageHeight", oRoot);
            dPageHeightOld = double.Parse(oNode.InnerText.Replace("cm", ""));
            oNode.InnerText = dPageHeight.ToString(".0") + "cm";

            oNode = GetNode("LeftMargin", oRoot);
            dMarginLeftOld = double.Parse(oNode.InnerText.Replace("cm", ""));
            oNode.InnerText = dMarginLeft.ToString(".0") + "cm";

            oNode = GetNode("RightMargin", oRoot);
            dMarginRightOld = double.Parse(oNode.InnerText.Replace("cm", ""));
            oNode.InnerText = dMarginRight.ToString(".0") + "cm";

            oNode = GetNode("TopMargin", oRoot);
            dMarginTopOld = double.Parse(oNode.InnerText.Replace("cm", ""));
            oNode.InnerText = dMarginTop.ToString(".0") + "cm";

            oNode = GetNode("BottomMargin", oRoot);
            dMarginBottomOld = double.Parse(oNode.InnerText.Replace("cm", ""));
            oNode.InnerText = dMarginBottom.ToString(".0") + "cm";
        }

        public void SetAllFieldWidth()
        {
            XmlNode oRoot = oXmlDoc.DocumentElement;
            XmlNode oNode = GetNode("ReportItems", oRoot, true);

            //Only change items in ReportItems
            if (oNode != null)
            {
                FixNodeWidth(oNode);
            }

            //Change the design area
            SetWidth(GetNode("Width", oRoot));
        }

        private void FixNodeWidth(XmlNode oRoot)
        {
            //check all nodes for a Width element
            foreach (XmlNode oNode in oRoot.ChildNodes)
            {
                //Change the width as a percentage of the orginal
                SetWidth(GetNode("Width", oNode));

                //Hack to able to also scale item in the middle of the page on the same line
                //Set in the Label field to ScaleLeft
                XmlNode oNodeLabel = GetNode("Label", oNode);

                if ((oNodeLabel != null) && (oNodeLabel.InnerText == "ScaleLeft"))
                {
                    //Change the Left as a percentage of the orginal
                    SetWidth(GetNode("Left", oNode));
                }

                if (oNode.HasChildNodes)
                {
                    FixNodeWidth(oNode);
                }
            }
        }

        private void SetWidth(XmlNode oNodeWidth)
        {
            if (oNodeWidth != null)
            {
                double dCurrentWidth = double.Parse(oNodeWidth.InnerText.Replace("cm", ""));
                double dNewWidth = (dCurrentWidth / (dPageWidthOld - dMarginLeftOld - dMarginRightOld)) * (dPageWidth - dMarginLeft - dMarginRight);

                oNodeWidth.InnerText = dNewWidth.ToString(".0") + "cm";
            }
        }

        private XmlNode GetNode(string sName, XmlNode oRoot)
        {
            return GetNode(sName, oRoot, false);
        }

        //Method to find the XML elements
        //This may not be the fastest way to do this?
        private XmlNode GetNode(string sName, XmlNode oRoot, bool bRecursive)
        {
            bool bFlag = false;
            int iCount = 0;
            XmlNode oNode = null;

            while ((!bFlag) && (iCount < oRoot.ChildNodes.Count))
            {
                oNode = oRoot.ChildNodes.Item(iCount);

                if (oNode.Name == sName)
                {
                    bFlag = true;
                }
                else if (bRecursive)
                {
                    if (oNode.HasChildNodes)
                    {
                        oNode = GetNode(sName, oNode, true);

                        if (oNode != null)
                        {
                            bFlag = true;
                        }
                    }
                }
                iCount++;
            }

            if (!bFlag)
            {
                oNode = null;
            }
            return oNode;
        }

        //Save the changed report
        public Stream GetReport()
        {
            MemoryStream ms = new MemoryStream();
            // Create Xml 
            XmlTextWriter writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
            oXmlDoc.Save(writer);
            ms.Position = 0;
            return ms;
        }
    }
}