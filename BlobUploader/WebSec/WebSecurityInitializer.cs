using System;
using WebMatrix.WebData;

namespace BlobUploader.WebSec
{
    public class WebSecurityInitializer
    {
        private WebSecurityInitializer() { }
        public static readonly WebSecurityInitializer Instance = new WebSecurityInitializer();
        private bool _isInitialized;
        private readonly object _syncRoot = new object();
        public void EnsureInitialize()
        {
            if (_isInitialized) return;
            lock (_syncRoot)
            {
                if (_isInitialized) return;
                _isInitialized = true;
                try
                {
                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
                
            }
        }
    }
}