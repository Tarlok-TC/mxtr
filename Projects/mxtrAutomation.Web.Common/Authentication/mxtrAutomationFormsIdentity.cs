using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Web.Security;

namespace mxtrAutomation.Web.Common.Authentication
{
    [Serializable]
    public class mxtrAutomationFormsIdentity : FormsIdentity, ImxtrAutomationIdentity, ISerializable
    {
        public string MxtrUserObjectID { get; set; }
        public string MxtrAccountObjectID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string SelectedWorkspaceIDs { get; set; }
        public string SharpspringUserName { get; set; }
        public string SharpspringPassword { get; set; }

        public mxtrAutomationFormsIdentity(FormsAuthenticationTicket ticket, string mxtrUserObjectID, string mxtrAccountObjectID, string userName, string fullName, string selectedWorkspaceIDs, string role, string sharpspringUserName, string sharpspringPassword)
            : base(ticket)
        {
            MxtrUserObjectID = mxtrUserObjectID;
            MxtrAccountObjectID = mxtrAccountObjectID;
            UserName = userName;
            FullName = fullName;
            SelectedWorkspaceIDs = selectedWorkspaceIDs;
            Role = role;
            SharpspringPassword = sharpspringPassword;
            SharpspringUserName = sharpspringUserName;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.State == StreamingContextStates.CrossAppDomain)
            {
                GenericIdentity gIdent = new GenericIdentity(this.Name, this.AuthenticationType);
                info.SetType(gIdent.GetType());

                MemberInfo[] serializableMembers = FormatterServices.GetSerializableMembers(gIdent.GetType());
                object[] serializableValues = FormatterServices.GetObjectData(gIdent, serializableMembers);

                for (int i = 0; i < serializableMembers.Length; i++)
                    info.AddValue(serializableMembers[i].Name, serializableValues[i]);
            }
            else
            {
                throw new InvalidOperationException("Serialization not supported");
            }
        }
    }
}
