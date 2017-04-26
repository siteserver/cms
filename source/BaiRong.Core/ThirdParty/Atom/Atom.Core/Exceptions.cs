/* 
  	* Exceptions.cs
	* [ part of Atom.NET library: http://atomnet.sourceforge.net ]
	* Author: Lawrence Oluyede
	* License: BSD-License (see below)
    
	Copyright (c) 2003, 2004 Lawrence Oluyede
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
    * this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
    * notice, this list of conditions and the following disclaimer in the
    * documentation and/or other materials provided with the distribution.
    * Neither the name of the copyright owner nor the names of its
    * contributors may be used to endorse or promote products derived from
    * this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
    ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
    LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
    CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
    SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
    INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
    ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
    POSSIBILITY OF SUCH DAMAGE.
*/
using System;

namespace Atom.Core
{
	#region Person constructs exception

	/// <summary>
	/// The exception that is thrown when a required element is not found in the feed or entry.
	/// </summary>
	[Serializable]
	public class RequiredElementNotFoundException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredElementNotFoundException"/> class.
		/// </summary>
		public RequiredElementNotFoundException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredElementNotFoundException"/> class with a specified error message.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		public RequiredElementNotFoundException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredElementNotFoundException"/> class with a specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.
		/// If the innerException parameter is not a null reference, the current exception is raised in a catch
		/// block that handles the inner exception. </param>
		public RequiredElementNotFoundException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// The exception that is thrown when a required attribute is not found in the feed or entry.
	/// </summary>
	[Serializable]
	public class RequiredAttributeNotFoundException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredElementNotFoundException"/> class.
		/// </summary>
		public RequiredAttributeNotFoundException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredElementNotFoundException"/> class with a specified error message.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		public RequiredAttributeNotFoundException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RequiredElementNotFoundException"/> class with a specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.
		/// If the innerException parameter is not a null reference, the current exception is raised in a catch
		/// block that handles the inner exception. </param>
		public RequiredAttributeNotFoundException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	#endregion Person constructs exception

	#region AtomContentCollection exception

	/// <summary>
	/// The exception that is thrown when a more than one multipart content is inserted in an entry.
	/// </summary>
	[Serializable]
	public class OnlyOneMultipartContentAllowedException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OnlyOneMultipartContentAllowedException"/> class.
		/// </summary>
		public OnlyOneMultipartContentAllowedException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OnlyOneMultipartContentAllowedException"/> class with a specified error message.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		public OnlyOneMultipartContentAllowedException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OnlyOneMultipartContentAllowedException"/> class with a specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.
		/// If the innerException parameter is not a null reference, the current exception is raised in a catch
		/// block that handles the inner exception. </param>
		public OnlyOneMultipartContentAllowedException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	#endregion AtomContentCollection exception

	#region AtomLinkCollection exceptions

	/// <summary>
	/// The exception that is thrown when the main link with alternate relationship is not inserted.
	/// </summary>
	[Serializable]
	public class MainAlternateLinkMissingException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MainAlternateLinkMissingException"/> class.
		/// </summary>
		public MainAlternateLinkMissingException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MainAlternateLinkMissingException"/> class with a specified error message.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		public MainAlternateLinkMissingException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MainAlternateLinkMissingException"/> class with a specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.
		/// If the innerException parameter is not a null reference, the current exception is raised in a catch
		/// block that handles the inner exception. </param>
		public MainAlternateLinkMissingException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	/// <summary>
	/// The exception that is thrown when two duplicate links are found.
	/// </summary>
	[Serializable]
	public class DuplicateLinkException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DuplicateLinkException"/> class.
		/// </summary>
		public DuplicateLinkException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DuplicateLinkException"/> class with a specified error message.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		public DuplicateLinkException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DuplicateLinkException"/> class with a specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="msg">A message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.
		/// If the innerException parameter is not a null reference, the current exception is raised in a catch
		/// block that handles the inner exception. </param>
		public DuplicateLinkException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}

	#endregion AtomLinkCollection exceptions
}
