// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
    public sealed class SignedXml
    {
        private Signature m_signature;

        private AsymmetricAlgorithm _signingKey;
        private XmlDocument _containingDocument = null;

        private bool[] _refProcessed = null;
        private int[] _refLevelCache = null;

        internal XmlElement _context = null;

        private Collection<string> _safeCanonicalizationMethods;

        //
        // public constant Url identifiers most frequently used within the XML Signature classes
        //

        public const string XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";

        public const string XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";
        public const string XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
        public const string XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

        public const string XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
        public const string XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

        //
        // public constructors
        //

        public SignedXml(XmlDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            Initialize(document.DocumentElement);
        }

        public SignedXml(XmlElement elem)
        {
            if (elem == null)
                throw new ArgumentNullException(nameof(elem));
            Initialize(elem);
        }

        private void Initialize(XmlElement element)
        {
            _containingDocument = (element == null ? null : element.OwnerDocument);
            _context = element;
            m_signature = new Signature();
            m_signature.SignedXml = this;
            m_signature.SignedInfo = new SignedInfo();
            _signingKey = null;

            _safeCanonicalizationMethods = new Collection<string>(new List<string> { XmlDsigC14NTransformUrl });
        }

        //
        // public properties
        //

        public AsymmetricAlgorithm SigningKey
        {
            get { return _signingKey; }
            set { _signingKey = value; }
        }

        public Signature Signature
        {
            get { return m_signature; }
        }

        public SignedInfo SignedInfo
        {
            get { return m_signature.SignedInfo; }
        }

        public X509Certificate2 KeyInfo
        {
            get { return m_signature.KeyInfo; }
            set { m_signature.KeyInfo = value; }
        }

        //
        // public methods
        //

        public void AddReference(Reference reference)
        {
            m_signature.SignedInfo.Reference = reference;
        }

        public void ComputeSignature()
        {
            BuildDigestedReferences();

            // Load the key
            AsymmetricAlgorithm key = SigningKey;

            // Check the signature algorithm associated with the key so that we can accordingly set the signature method
            if (SignedInfo.SignatureMethod == null)
            {
                if (key is RSA)
                {
                    // Default to RSA-SHA1
                    SignedInfo.SignatureMethod = XmlDsigRSASHA1Url;
                }
                else
                {
                    throw new CryptographicException();
                }
            }

            // See if there is a signature description class defined in the Config file
            RSASignatureDescription signatureDescription = CryptoHelpers.CreateFromName(SignedInfo.SignatureMethod) as RSASignatureDescription;
            HashAlgorithm hashAlg = signatureDescription.CreateDigest();
            byte[] hashvalue = GetC14NDigest(hashAlg);
            RSAPKCS1SignatureFormatter asymmetricSignatureFormatter = signatureDescription.CreateFormatter(key);
            m_signature.SignatureValue = asymmetricSignatureFormatter.CreateSignature(hashvalue);
        }


        //
        // private methods
        //

        private byte[] GetC14NDigest(HashAlgorithm hash)
        {
            bool isKeyedHashAlgorithm = hash is KeyedHashAlgorithm;
            string baseUri = (_containingDocument == null ? null : _containingDocument.BaseURI);
            XmlDocument doc = Utils.PreProcessElementInput(SignedInfo.GetXml());

            // Add non default namespaces in scope
            CanonicalXmlNodeList namespaces = (_context == null ? null : Utils.GetPropagatedAttributes(_context));
            Utils.AddNamespaces(doc.DocumentElement, namespaces);

            Transform c14nMethodTransform = SignedInfo.CanonicalizationMethodObject;

            c14nMethodTransform.LoadInput(doc);
            return c14nMethodTransform.GetDigestedOutput(hash);
        }

        private void BuildDigestedReferences()
        {
            // Default the DigestMethod and Canonicalization
            Reference reference = SignedInfo.Reference;

            CanonicalXmlNodeList nodeList = new CanonicalXmlNodeList();
            // If no DigestMethod has yet been set, default it to sha1
            if (reference.DigestMethod == null)
                reference.DigestMethod = XmlDsigSHA1Url;

            reference.UpdateHashValue(_containingDocument, nodeList);
        }
    }
}
