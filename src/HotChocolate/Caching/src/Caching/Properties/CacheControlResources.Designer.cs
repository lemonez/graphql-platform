﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotChocolate.Caching.Properties {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CacheControlResources {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CacheControlResources() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("HotChocolate.Caching.Properties.CacheControlResources", typeof(CacheControlResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string CacheControlDirectiveType_Description {
            get {
                return ResourceManager.GetString("CacheControlDirectiveType_Description", resourceCulture);
            }
        }
        
        internal static string CacheControlDirectiveType_InheritMaxAge {
            get {
                return ResourceManager.GetString("CacheControlDirectiveType_InheritMaxAge", resourceCulture);
            }
        }
        
        internal static string CacheControlDirectiveType_MaxAge {
            get {
                return ResourceManager.GetString("CacheControlDirectiveType_MaxAge", resourceCulture);
            }
        }
        
        internal static string CacheControlDirectiveType_SharedMaxAge {
            get {
                return ResourceManager.GetString("CacheControlDirectiveType_SharedMaxAge", resourceCulture);
            }
        }
        
        internal static string CacheControlDirectiveType_Vary {
            get {
                return ResourceManager.GetString("CacheControlDirectiveType_Vary", resourceCulture);
            }
        }
        
        internal static string CacheControlDirectiveType_Scope {
            get {
                return ResourceManager.GetString("CacheControlDirectiveType_Scope", resourceCulture);
            }
        }
        
        internal static string CacheControlScopeType_Description {
            get {
                return ResourceManager.GetString("CacheControlScopeType_Description", resourceCulture);
            }
        }
        
        internal static string CacheControlScopeType_Private {
            get {
                return ResourceManager.GetString("CacheControlScopeType_Private", resourceCulture);
            }
        }
        
        internal static string CacheControlScopeType_Public {
            get {
                return ResourceManager.GetString("CacheControlScopeType_Public", resourceCulture);
            }
        }
        
        internal static string ErrorHelper_CacheControlBothMaxAgeAndInheritMaxAge {
            get {
                return ResourceManager.GetString("ErrorHelper_CacheControlBothMaxAgeAndInheritMaxAge", resourceCulture);
            }
        }
        
        internal static string ErrorHelper_CacheControlBothSharedMaxAgeAndInheritMaxAge {
            get {
                return ResourceManager.GetString("ErrorHelper_CacheControlBothSharedMaxAgeAndInheritMaxAge", resourceCulture);
            }
        }
        
        internal static string ErrorHelper_CacheControlInheritMaxAgeOnQueryTypeField {
            get {
                return ResourceManager.GetString("ErrorHelper_CacheControlInheritMaxAgeOnQueryTypeField", resourceCulture);
            }
        }
        
        internal static string ErrorHelper_CacheControlInheritMaxAgeOnType {
            get {
                return ResourceManager.GetString("ErrorHelper_CacheControlInheritMaxAgeOnType", resourceCulture);
            }
        }
        
        internal static string ErrorHelper_CacheControlNegativeMaxAge {
            get {
                return ResourceManager.GetString("ErrorHelper_CacheControlNegativeMaxAge", resourceCulture);
            }
        }
        
        internal static string ErrorHelper_CacheControlNegativeSharedMaxAge {
            get {
                return ResourceManager.GetString("ErrorHelper_CacheControlNegativeSharedMaxAge", resourceCulture);
            }
        }
        
        internal static string ErrorHelper_CacheControlOnInterfaceField {
            get {
                return ResourceManager.GetString("ErrorHelper_CacheControlOnInterfaceField", resourceCulture);
            }
        }
        
        internal static string ThrowHelper_EncounteredIntrospectionField {
            get {
                return ResourceManager.GetString("ThrowHelper_EncounteredIntrospectionField", resourceCulture);
            }
        }
        
        internal static string ThrowHelper_UnexpectedCacheControlScopeValue {
            get {
                return ResourceManager.GetString("ThrowHelper_UnexpectedCacheControlScopeValue", resourceCulture);
            }
        }
    }
}
