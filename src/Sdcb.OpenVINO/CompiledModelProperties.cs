using Sdcb.OpenVINO.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdcb.OpenVINO;

using static NativeMethods;

internal unsafe class CompiledModelProperties : IDictionary<string, string>
{
    private readonly ov_compiled_model* _ptr;

    public CompiledModelProperties(ov_compiled_model* ptr) { _ptr = ptr; }

    public void ThrowIfDisposed()
    {
        if (_ptr == null) throw new ObjectDisposedException(nameof(CompiledModelProperties));
    }

    public string this[string key] 
    {
        get
        {
            if (!TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException($"key: {key}");
            }

            return value;
        }
        set
        {
            ThrowIfDisposed();

            fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key + '\0'))
            fixed (byte* valuePtr = Encoding.UTF8.GetBytes(value + '\0'))
            {
                OpenVINOException.ThrowIfFailed(ov_compiled_model_set_property(_ptr, __arglist(keyPtr, valuePtr)));
            }
        }
    }

    public ICollection<string> Keys => this["SUPPORTED_PROPERTIES"].Split(' ');

    public ICollection<string> Values => Keys.Select(x => this[x]).ToList();

    public int Count => Keys.Count;

    public bool IsReadOnly => false;

    public void Add(string key, string value)
    {
        throw new NotSupportedException();
    }

    public void Add(KeyValuePair<string, string> item)
    {
        throw new NotSupportedException();
    }

    public void Clear()
    {
        throw new NotSupportedException();
    }

    public bool Contains(KeyValuePair<string, string> item)
    {
        return ContainsKey(item.Key);
    }

    public bool ContainsKey(string key)
    {
        return Keys.Contains(key);
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("The number of elements in the source Dictionary is greater than the available space from arrayIndex to the end of the destination array.");

        int i = 0;
        foreach (KeyValuePair<string, string> property in this)
        {
            if (i >= Count) break;
            array[arrayIndex + i] = property;
            i++;
        }
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        foreach (string key in Keys)
        {
            yield return new KeyValuePair<string, string>(key, this[key]);
        }
    }

    public bool Remove(string key)
    {
        throw new NotSupportedException();
    }

    public bool Remove(KeyValuePair<string, string> item)
    {
        throw new NotSupportedException();
    }

    public bool TryGetValue(string key, out string value)
    {
        ThrowIfDisposed();

        byte* valuePtr;
        fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key + '\0'))
        {
            ov_status_e status = ov_compiled_model_get_property(_ptr, keyPtr, &valuePtr);
            if (status == ov_status_e.GENERAL_ERROR)
            {
                value = null!;
                return false;
            }
            else
            {
                OpenVINOException.ThrowIfFailed(status);
            }

            try
            {
                value = StringUtils.UTF8PtrToString((IntPtr)valuePtr)!;
                return true;
            }
            finally
            {
                ov_free(valuePtr);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
