using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MCCard
{
    public class SqlCommand
    {
        #region Select
        /// <summary>
        /// Select
        /// </summary>
        public class Select
        {
            #region UserByUSN()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="USN"></param>
            /// <param name="Password"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string UserByUSN(string USN, string Password, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select * From Customers Where USN = @USN";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "USN"), USN));

                if (!String.IsNullOrEmpty(Password))
                {
                    strSql += " And Password=@Password";

                    Parameters.Add(new SqlParameter(string.Format("@{0}", "Password"), Password.EncryptDES()));
                }
                return string.Format(strSql);
            }
            #endregion

            #region UserByUID()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="UID"></param>
            /// <param name="Password"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string UserByUID(string UID, string Password, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select * From Customers Where UID = @UID";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), UID.EncryptDES()));

                if (!String.IsNullOrEmpty(Password))
                {
                    strSql += " And Password=@Password";

                    Parameters.Add(new SqlParameter(string.Format("@{0}", "Password"), Password.EncryptDES()));
                }
                return string.Format(strSql);
            }
            #endregion

            #region UserFacebook()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="UID"></param>
            /// <param name="Password"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string UserFacebook(string UID, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select * From Customers Where FacebookUID = @FacebookUID";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "FacebookUID"), UID.EncryptDES()));

                return string.Format(strSql);
            }
            #endregion

            #region UserGoogle()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="UID"></param>
            /// <param name="Password"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string UserGoogle(string UID, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select * From Customers Where GooglePlusUID = @GooglePlusUID";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "GooglePlusUID"), UID.EncryptDES()));

                return string.Format(strSql);
            }
            #endregion

            #region HomeBanner()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string HomeBanner()
            {
                string strSql = "Select * From HomeBanner Where 1 = 1 And Status = 0 And (Type = 0 Or Type = 4)  Order By Type Asc, Seq Asc";

                return string.Format(strSql);
            }
            #endregion

            #region HomeSubBanner()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string HomeSubBanner(string Type, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select *\n"
                    + "From  HomeBanner\n"
                    + "Where 1 = 1\n"
                    + "And Status = 0\n"
                    + "And Type = @Type\n"
                    + "Order By Seq, LastModifyTime Desc\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Type"), Type));

                return string.Format(strSql);
            }
            #endregion

            #region ExclusiveDiscountBanner()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string ExclusiveDiscountBanner()
            {
                string strSql = "Select Top 1 * From HomeBanner Where 1 = 1 And Status = 0 And Type = 2  Order By Seq, LastModifyTime Desc";

                return string.Format(strSql);
            }
            #endregion

            #region ExclusiveDiscountSubBanner()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string ExclusiveDiscountSubBanner()
            {
                string strSql = "Select Top 2 * From HomeBanner Where 1 = 1 And Status = 0 And Type = 3  Order By Seq, LastModifyTime Desc";

                return string.Format(strSql);
            }
            #endregion

            #region LatestNewsBannerPC()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string LatestNewsBannerPC()
            {
                string strSql = "Select Top 3 * From HomeBanner Where 1 = 1 And Status = 0 And Type = 11  Order By Seq, LastModifyTime Desc";

                return string.Format(strSql);
            }
            #endregion

            #region LatestNewsBannerMB()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string LatestNewsBannerMB()
            {
                string strSql = "Select Top 3 * From HomeBanner Where 1 = 1 And Status = 0 And Type = 12  Order By Seq, LastModifyTime Desc";

                return string.Format(strSql);
            }
            #endregion

            #region News()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string News()
            {
                string strSql = "Select Top 3 *\n"
                    + "From  News\n"
                    + "Where 1 = 1\n"
                    + "And Status = 0\n"
                    + "Order By Seq, LastModifyTime Desc\n";

                return string.Format(strSql);
            }
            #endregion

            #region ExhibitorsNews()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string ExhibitorsNews(string Type, ref List<IDataParameter> Parameters)
            {
                StringBuilder where = new StringBuilder();

                string strSql = "Select Top 4 *\n"
                    + "From  ExhibitorsNews\n"
                    + "Where 1 = 1\n"
                    + "And Status = 0\n"
                    + "{0}"
                    + "Order By Type, Seq, LastModifyTime Desc\n";

                if (!String.IsNullOrEmpty(Type))
                {
                    where.AppendLine(string.Format("And Type = @Type"));

                    Parameters = new List<IDataParameter>();
                    Parameters.Add(new SqlParameter(string.Format("@{0}", "Type"), Type));
                }
                return string.Format(strSql, where.ToString());
            }
            #endregion

            #region EmailVerify()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="USN"></param>
            /// <param name="Mail"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string EmailVerify(string USN, string Mail, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select * From EmailVerify Where USN = @USN And Mail=@Mail";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "USN"), USN));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Mail"), Mail.EncryptDES()));

                return string.Format(strSql);
            }
            #endregion

            #region CardStyleByCardNumber()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="CardNumber"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string CardStyleByCardNumber(string CardNumber, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select UID\n"
                    + "	,StartNumber\n"
                    + "	,EndNumber\n"
                    + "	,Status\n"
                    + "	,Seq\n"
                    + "	,IconPath\n"
                    + "	,Subject\n"
                    + "	,Body\n"
                    + "	,Pricing\n"
                    + "	,LastModifyTime\n"
                    + "From CardStyle\n"
                    + "Where StartNumber <= @CardNumber\n"
                    + "And EndNumber >= @CardNumber\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "CardNumber"), CardNumber));

                return string.Format(strSql);
            }
            #endregion

            #region PointCard()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string PointCard(string UID, string CardNumber, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select\n"
                    + "	pc.UID\n"
                    + "	,(Select ct.UID From Customers ct Where ct.USN = pc.CustID) As PhoneNumber\n"
                    + "	,pc.CustID\n"
                    + "	,pc.CardNumber\n"
                    + "	,pc.SecurityCode\n"
                    + "	,pc.LastModifyTime\n"
                    + "	,IsNull(cs.IconPath, '') As IconPath\n"
                    + "From PointCard pc\n"
                    + "Left Join CardStyle cs On (1 = 1\n"
                    + "	And cs.StartNumber <= pc.CardNumber\n"
                    + "	And cs.EndNumber >= pc.CardNumber)\n"
                    + "Where 1 = 1\n";

                Parameters = new List<IDataParameter>();

                if (!String.IsNullOrEmpty(UID))
                {
                    strSql += "And pc.UID = @UID\n";

                    Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), UID));
                }
                if (!String.IsNullOrEmpty(CardNumber))
                {
                    strSql += "And pc.CardNumber = @CardNumber\n";

                    Parameters.Add(new SqlParameter(string.Format("@{0}", "CardNumber"), CardNumber));
                }
                strSql += "Order By pc.LastModifyTime Desc\n";

                return strSql;
            }
            #endregion

            #region PointCard()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string PointCard(string USN, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select\n"
                    + "	pc.UID\n"
                    + "	,(Select ct.UID From Customers ct Where ct.USN = pc.CustID) As PhoneNumber\n"
                    + "	,pc.CustID\n"
                    + "	,pc.CardNumber\n"
                    + "	,pc.SecurityCode\n"
                    + "	,pc.LastModifyTime\n"
                    + "	,IsNull(cs.IconPath, '') As IconPath\n"
                    + "From PointCard pc\n"
                    + "Left Join CardStyle cs On (1 = 1\n"
                    + "	And cs.StartNumber <= pc.CardNumber\n"
                    + "	And cs.EndNumber >= pc.CardNumber)\n"
                    + "Where 1 = 1\n"
                    + "And pc.CustID = @CustID\n"
                    + "Order By pc.LastModifyTime Desc\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "CustID"), USN));

                return strSql;
            }
            #endregion

            #region CommonProblemType()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string CommonProblemType()
            {
                string strSql = "Select Top 4 UID\n"
                    + "	,TypeName\n"
                    + "	,Status\n"
                    + "	,Seq\n"
                    + "	,LastModifyTime\n"
                    + "From CommonProblemType\n"
                    + "Where 1 = 1\n"
                    + "And Status = 0\n"
                    + "Order By Seq, LastModifyTime Desc\n";

                return strSql;
            }
            #endregion

            #region CommonProblemDetail()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string CommonProblemDetail()
            {
                string strSql = "Select UID\n"
                    + "	,TypeID\n"
                    + "	,Status\n"
                    + "	,Question\n"
                    + "	,Answer\n"
                    + "	,Seq\n"
                    + "	,LastModifyTime\n"
                    + "From CommonProblemDetail\n"
                    + "Where 1 = 1\n"
                    + "Order By TypeID, Seq, LastModifyTime Desc\n";

                return strSql;
            }
            #endregion

            #region CardStyle()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string CardStyle()
            {
                string strSql = "Select UID\n"
                    + "	,StartNumber\n"
                    + "	,EndNumber\n"
                    + "	,Status\n"
                    + "	,Seq\n"
                    + "	,IconPath\n"
                    + "	,Subject\n"
                    + "	,Body\n"
                    + "	,Pricing\n"
                    + "	,Replace(Convert(NVarchar, LastModifyTime, 120), '-', '/') As LastModifyTime\n"
                    + "From CardStyle\n"
                    + "Where 1 = 1\n"
                    + "Order By Seq, LastModifyTime Desc\n";

                return strSql;
            }
            #endregion

            #region CardStyle()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string CardStyle(string UID, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select UID\n"
                    + "	,StartNumber\n"
                    + "	,EndNumber\n"
                    + "	,Status\n"
                    + "	,Seq\n"
                    + "	,IconPath\n"
                    + "	,Subject\n"
                    + "	,Body\n"
                    + "	,Pricing\n"
                    + "	,Replace(Convert(NVarchar, LastModifyTime, 120), '-', '/') As LastModifyTime\n"
                    + "From CardStyle\n"
                    + "Where 1 = 1\n"
                    + "And UID = @UID\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), UID));

                return strSql;
            }
            #endregion

            #region SMSVerify()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string SMSVerify(string UID, string Code, ref List<IDataParameter> Parameters)
            {
                string strSql = "Select *\n"
                    + "From SMSVerify\n"
                    + "Where 1 = 1\n"
                    + "And UID = @UID\n"
                    + "And Code = @Code\n"
                    + "And Status = 0\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), UID));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Code"), Code));

                return strSql;
            }
            #endregion
        }
        #endregion

        #region Update
        /// <summary>
        /// Update
        /// </summary>
        public class Update
        {
            #region UserMail()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="USN"></param>
            /// <param name="Mail"></param>
            /// <param name="Status"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string UserMail(string USN, string Mail, string Status, ref List<IDataParameter> Parameters)
            {
                string strSql = "Update Customers Set\n"
                    + "  Mail = @Mail\n"
                    + "  ,Status = @Status\n"
                    + "  ,LastModifyTime = GetDate()\n"
                    + "Where 1 = 1\n"
                    + "And USN = @USN\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "USN"), USN));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Mail"), Mail.EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Status"), Status));

                return strSql;
            }
            #endregion

            #region UserRegister()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="UID"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string UserRegister(string UID, ref List<IDataParameter> Parameters)
            {
                string strSql = "Update Customers Set\n"
                    + "  Status = 0\n"
                    + "  ,LastModifyTime = GetDate()\n"
                    + "Where 1 = 1\n"
                    + "And UID = @UID\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), UID));

                return strSql;
            }
            #endregion

            #region User()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string User(Hashtable Data, ref List<IDataParameter> Parameters)
            {
                string strSql = "Update Customers Set\n"
                    + "	Password = @Password\n"
                    + "	,LastName = @LastName\n"
                    + "	,FirstName = @FirstName\n"
                    + "	,Sex = @Sex\n"
                    + "	,Zipcode = @Zipcode\n"
                    + "	,City = @City\n"
                    + "	,Township = @Township\n"
                    + "	,Address = @Address\n"
                    + "	,MaritalStatus = @MaritalStatus\n"
                    + "	,Children = @Children\n"
                    + "	,ChildrenYear = @ChildrenYear\n"
                    + "	,Mail = @Mail\n"
                    + "	,ReceiveNews = @ReceiveNews\n"
                    + "	,LastModifyTime = GetDate()\n"
                    + "Where 1 = 1\n"
                    + "And USN = @USN\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "USN"), Data["USN"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Password"), Data["Password"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "LastName"), Data["LastName"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "FirstName"), Data["FirstName"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Sex"), Data["Sex"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Zipcode"), Data["Zipcode"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "City"), Data["City"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Township"), Data["Township"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Address"), Data["Address"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "MaritalStatus"), Data["MaritalStatus"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Children"), Data["Children"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "ChildrenYear"), Data["ChildrenYear"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Mail"), Data["Mail"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "ReceiveNews"), Data["ReceiveNews"].ToString()));

                return strSql;
            }
            #endregion

            #region UserFacebook()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="USN"></param>
            /// <param name="FacebookUID"></param>
            /// <param name="FacebookName"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string UserFacebook(string USN, string FacebookUID, string FacebookName, ref List<IDataParameter> Parameters)
            {
                string strSql = "Update Customers Set FacebookUID = @FacebookUID, fb_user_id = @FacebookUID, fb_username = @FacebookName, LastModifyTime = GetDate() Where USN=@USN";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "USN"), USN));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "FacebookUID"), FacebookUID.EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "FacebookName"), FacebookName.EncryptDES()));

                return string.Format(strSql);
            }
            #endregion

            #region UserGoogle()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="USN"></param>
            /// <param name="GooglePlusUID"></param>
            /// <param name="GooglePlusName"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string UserGoogle(string USN, string GooglePlusUID, string GooglePlusName, ref List<IDataParameter> Parameters)
            {
                string strSql = "Update Customers Set GooglePlusUID = @GooglePlusUID, gplus_user_id = @GooglePlusUID, gplus_username = @GooglePlusName, LastModifyTime = GetDate() Where USN=@USN";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "USN"), USN));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "GooglePlusUID"), GooglePlusUID.EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "GooglePlusName"), GooglePlusName.EncryptDES()));

                return string.Format(strSql);
            }
            #endregion

            #region PointCardCardStyleID()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string PointCardCardStyleID(string UID, string CardStyleID, ref List<IDataParameter> Parameters)
            {
                string strSql = "Update PointCard Set\n"
                    + "	CardStyleID = @CardStyleID\n"
                    + "	,LastModifyTime = GetDate()\n"
                    + "From PointCard\n"
                    + "Where 1 = 1\n"
                    + "And UID = @UID\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), UID));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "CardStyleID"), CardStyleID));

                return strSql;
            }
            #endregion
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete
        /// </summary>
        public class Delete
        {
            #region PartnerApply()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string PartnerApply(string UID, string CancelComments, string IP, ref List<IDataParameter> Parameters)
            {
                string strSql = "Update myparty.partnerapply Set\n"
                    + "  Status = 99\n"
                    + "  ,BookingTime = null\n"
                    + "  ,IP = @IP\n"
                    + "  ,CancelTime = now()\n"
                    + "  ,CancelComments = @CancelComments\n"
                    + "Where UID = @UID\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), UID));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "IP"), IP));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "CancelComments"), CancelComments));

                return strSql;
            }
            #endregion

            #region PointCard()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="UID"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string PointCard(string UID, ref List<IDataParameter> Parameters)
            {
                string strSql = "Delete From PointCard Where UID = @UID\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), UID));

                return strSql;
            }
            #endregion

            #region PointCard()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="USN"></param>
            /// <param name="CardNumber"></param>
            /// <param name="Parameters"></param>
            /// <returns></returns>
            public string PointCard(string USN, string CardNumber, ref List<IDataParameter> Parameters)
            {
                string strSql = "Delete From PointCard Where CustID = @CustID And CardNumber = @CardNumber\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "CustID"), USN));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "CardNumber"), CardNumber));

                return strSql;
            }
            #endregion
        }
        #endregion

        #region Insert
        /// <summary>
        /// Insert
        /// </summary>
        public class Insert
        {
            #region User()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string User(Hashtable Data, ref List<IDataParameter> Parameters)
            {
                string strSql = "Insert Into Customers (\n"
                    + "	USN\n"
                    + "	,UID\n"
                    + "	,Password\n"
                    + "	,LastName\n"
                    + "	,FirstName\n"
                    + "	,Sex\n"
                    + "	,Birthday\n"
                    + "	,Zipcode\n"
                    + "	,City\n"
                    + "	,Township\n"
                    + "	,Address\n"
                    + "	,MaritalStatus\n"
                    + "	,Children\n"
                    + "	,ChildrenYear\n"
                    + "	,Mail\n"
                    + "	,ReceiveNews\n"
                    + "	,Status\n"
                    + "	,LastModifyTime\n"
                    + "	,Source\n"
                    + ")\n"
                    + "Values (\n"
                    + "	@USN\n"
                    + "	,@UID\n"
                    + "	,@Password\n"
                    + "	,@LastName\n"
                    + "	,@FirstName\n"
                    + "	,@Sex\n"
                    + "	,@Birthday\n"
                    + "	,@Zipcode\n"
                    + "	,@City\n"
                    + "	,@Township\n"
                    + "	,@Address\n"
                    + "	,@MaritalStatus\n"
                    + "	,@Children\n"
                    + "	,@ChildrenYear\n"
                    + "	,@Mail\n"
                    + "	,@ReceiveNews\n"
                    + "	,0\n"
                    + "	,GetDate()\n"
                    + "	,'WEB'\n"
                    + ")\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "USN"), Data["USN"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), Data["UID"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Password"), Data["Password"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "LastName"), Data["LastName"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "FirstName"), Data["FirstName"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Sex"), Data["Sex"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Birthday"), Data["Birthday"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Zipcode"), Data["Zipcode"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "City"), Data["City"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Township"), Data["Township"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Address"), Data["Address"] != null ? Data["Address"].ToString().EncryptDES() : ""));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "MaritalStatus"), Data["MaritalStatus"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Children"), Data["Children"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "ChildrenYear"), Data["ChildrenYear"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Mail"), Data["Mail"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "ReceiveNews"), Data["ReceiveNews"].ToString()));

                return strSql;
            }
            #endregion

            #region PointCard()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string PointCard(Hashtable Data, ref List<IDataParameter> Parameters)
            {
                string strSql = "Insert Into PointCard (\n"
                    + "	UID\n"
                    + "	,CustID\n"
                    + "	,CardNumber\n"
                    + "	,SecurityCode\n"
                    + "	,Source\n"
                    + "	,LastModifyTime\n"
                    + ")\n"
                    + "Values (\n"
                    + "	@UID\n"
                    + "	,@CustID\n"
                    + "	,@CardNumber\n"
                    + "	,@SecurityCode\n"
                    + "	,@Source\n"
                    + "	,GetDate()\n"
                    + ")\n";

                Parameters = new List<IDataParameter>();
                Parameters.Add(new SqlParameter(string.Format("@{0}", "UID"), Data["UID"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "CustID"), Data["CustID"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "CardNumber"), Data["CardNumber"].ToString()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "SecurityCode"), Data["SecurityCode"].ToString().EncryptDES()));
                Parameters.Add(new SqlParameter(string.Format("@{0}", "Source"), Data["Source"].ToString()));
                return strSql;
            }
            #endregion
        }
        #endregion
    }
}