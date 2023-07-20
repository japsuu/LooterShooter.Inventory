using System;
using UnityEngine;

namespace LooterShooter.Framework.Guids
{
    /// <summary>
    /// Serializable wrapper for System.Guid.
    /// Can be implicitly converted to/from System.Guid.
    ///
    /// Author: Katie Kennedy (Searous) https://github.com/Searous/Unity.SerializableGuid.
    /// </summary>
    [Serializable]
    public struct SerializableGuid : ISerializationCallbackReceiver
    {
        private Guid _guid;
        [SerializeField] private string _serializedGuid;


        public SerializableGuid(Guid guid)
        {
            _guid = guid;
            _serializedGuid = null;
        }


        public override bool Equals(object obj)
        {
            return obj is SerializableGuid guid && _guid.Equals(guid._guid);
        }


        public override int GetHashCode()
        {
            return -1324198676 + _guid.GetHashCode();
        }


        public void OnAfterDeserialize()
        {
            try
            {
                _guid = Guid.Parse(_serializedGuid);
            }
            catch
            {
                _guid = Guid.Empty;
                Debug.LogWarning($"Attempted to parse invalid GUID string '{_serializedGuid}'. GUID will set to System.Guid.Empty");
            }
        }


        public void OnBeforeSerialize()
        {
            _serializedGuid = _guid.ToString();
        }


        public override string ToString()
        {
            return _guid.ToString();
        }


        public static bool operator ==(SerializableGuid a, SerializableGuid b)
        {
            return a._guid == b._guid;
        }


        public static bool operator !=(SerializableGuid a, SerializableGuid b)
        {
            return a._guid != b._guid;
        }


        public static implicit operator SerializableGuid(Guid guid)
        {
            return new SerializableGuid(guid);
        }


        public static implicit operator Guid(SerializableGuid serializable)
        {
            return serializable._guid;
        }


        public static implicit operator SerializableGuid(string serializedGuid)
        {
            return new SerializableGuid(Guid.Parse(serializedGuid));
        }


        public static implicit operator string(SerializableGuid serializedGuid)
        {
            return serializedGuid.ToString();
        }
    }
}