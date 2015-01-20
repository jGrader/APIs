﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GraderApi.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GraderApi.Resources.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You cannot change the scope of this team! Please delete it and create a new one for the intended entity..
        /// </summary>
        internal static string CannotChangeEntityForTeam {
            get {
                return ResourceManager.GetString("CannotChangeEntityForTeam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The server was unable to establish a connection to LDAP.
        /// </summary>
        internal static string CannotConnectToLdap {
            get {
                return ResourceManager.GetString("CannotConnectToLdap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You cannot remove all members of the old team!.
        /// </summary>
        internal static string CannotDeleteAllMembers {
            get {
                return ResourceManager.GetString("CannotDeleteAllMembers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are no longer allowed to ask for excuses for this course!.
        /// </summary>
        internal static string ExcuseNumberExceeded {
            get {
                return ResourceManager.GetString("ExcuseNumberExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided session ID has expired. Please log in again..
        /// </summary>
        internal static string ExpiredSessionId {
            get {
                return ResourceManager.GetString("ExpiredSessionId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are no longer allowed to ask for extensions for this course!.
        /// </summary>
        internal static string ExtensionNumberExceeded {
            get {
                return ResourceManager.GetString("ExtensionNumberExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The uploaded files and the expected files to be uploaded ... IDK, something is wrong.
        /// </summary>
        internal static string FileNamesDoNotMatch {
            get {
                return ResourceManager.GetString("FileNamesDoNotMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of uploaded files is not consistent with the number of expected files..
        /// </summary>
        internal static string FileNumberDoesNotMatch {
            get {
                return ResourceManager.GetString("FileNumberDoesNotMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was an internal error. Please try again!.
        /// </summary>
        internal static string InternalDatabaseError {
            get {
                return ResourceManager.GetString("InternalDatabaseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The courseId provided is invalid..
        /// </summary>
        internal static string InvalidCourse {
            get {
                return ResourceManager.GetString("InvalidCourse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The entered username and/or password do not match!.
        /// </summary>
        internal static string InvalidCredentials {
            get {
                return ResourceManager.GetString("InvalidCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The courseUserId provided is invalid..
        /// </summary>
        internal static string InvalidEnrollment {
            get {
                return ResourceManager.GetString("InvalidEnrollment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The entityId provided is invalid.
        /// </summary>
        internal static string InvalidEntity {
            get {
                return ResourceManager.GetString("InvalidEntity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The gradeComponentId is invalid..
        /// </summary>
        internal static string InvalidGradeComponentId {
            get {
                return ResourceManager.GetString("InvalidGradeComponentId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The request is invalid - please make sure that you are using an authorized client..
        /// </summary>
        internal static string InvalidRequest {
            get {
                return ResourceManager.GetString("InvalidRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are accessing an invalid route. How?.
        /// </summary>
        internal static string InvalidRoute {
            get {
                return ResourceManager.GetString("InvalidRoute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided session ID is invalid.
        /// </summary>
        internal static string InvalidSessionId {
            get {
                return ResourceManager.GetString("InvalidSessionId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The submissionId is invalid.
        /// </summary>
        internal static string InvalidSubmissionId {
            get {
                return ResourceManager.GetString("InvalidSubmissionId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided taskId is invalid.
        /// </summary>
        internal static string InvalidTaskId {
            get {
                return ResourceManager.GetString("InvalidTaskId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The request does not contain any files.
        /// </summary>
        internal static string NoFiles {
            get {
                return ResourceManager.GetString("NoFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user is not part of any team!.
        /// </summary>
        internal static string NoTeamFound {
            get {
                return ResourceManager.GetString("NoTeamFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTPS is required for accessing this resource.
        /// </summary>
        internal static string NotHttps {
            get {
                return ResourceManager.GetString("NotHttps", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested feature is not available. Make a feature request!.
        /// </summary>
        internal static string NotImplemented {
            get {
                return ResourceManager.GetString("NotImplemented", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You do not have permission to access this resource.
        /// </summary>
        internal static string PermissionsInsufficient {
            get {
                return ResourceManager.GetString("PermissionsInsufficient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot convert one of the strings to enum.
        /// </summary>
        internal static string PermissionsParseError {
            get {
                return ResourceManager.GetString("PermissionsParseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user {0} is already a member of other team for this entity!.
        /// </summary>
        internal static string TeamMemberAlreadyInTeam {
            get {
                return ResourceManager.GetString("TeamMemberAlreadyInTeam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unexpected error turned up. Please try again!.
        /// </summary>
        internal static string UnexpectedError {
            get {
                return ResourceManager.GetString("UnexpectedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The request does not have mime/form-data enctype!.
        /// </summary>
        internal static string UnsupportedMediaType {
            get {
                return ResourceManager.GetString("UnsupportedMediaType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user was not found in the system.
        /// </summary>
        internal static string UserNotFound {
            get {
                return ResourceManager.GetString("UserNotFound", resourceCulture);
            }
        }
    }
}
