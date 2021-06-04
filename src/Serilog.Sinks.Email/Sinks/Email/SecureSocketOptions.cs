//
// SecureSocketOptions.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2021 .NET Foundation and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if MAIL_KIT

namespace Serilog.Sinks.Email
{
    /// <summary>Secure socket options.</summary>
    /// <remarks>
    /// Provides a way of specifying the SSL and/or TLS encryption that
    /// should be used for a connection.
    /// </remarks>
    public enum SecureSocketOptions
    {
        /// <summary>
        /// No SSL or TLS encryption should be used.
        /// </summary>
        None = 0,

        /// <summary>
        /// Allow the implementation to decide which SSL or TLS options to use (default).
        /// If the server does not support SSL or TLS, then the connection will continue
        /// without any encryption.
        /// </summary>
        Auto = 1,

        /// <summary>
        /// The connection should use SSL or TLS encryption immediately.
        /// </summary>
        SslOnConnect = 2,

        /// <summary>
        /// Elevates the connection to use TLS encryption immediately after reading the greeting
        /// and capabilities of the server.
        /// </summary>
        StartTls = 3,

        /// <summary>
        /// Elevates the connection to use TLS encryption immediately after reading the greeting
        /// and capabilities of the server, but only if the server supports the STARTTLS
        /// extension.
        /// </summary>
        StartTlsWhenAvailable = 4
    }
}

#endif 
