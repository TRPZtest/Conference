﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Conference.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password, string salt = "")
        {
            if (String.IsNullOrEmpty(password))
            {
                return String.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }
    }
}