#pragma once
#include <oaidl.h>

namespace ArnoldVinkCode
{
	inline VARIANT bstr_to_variant(BSTR value)
	{
		VARIANT variant;
		VariantInit(&variant);
		variant.vt = VT_BSTR;
		variant.bstrVal = value;
		//VariantClear(&variant);
		return variant;
	}

	inline VARIANT wchar_to_variant(const wchar_t* value)
	{
		VARIANT variant;
		VariantInit(&variant);
		variant.vt = VT_BSTR;
		variant.bstrVal = SysAllocString(value);
		//SysFreeString(variant.bstrVal);
		//VariantClear(&variant);
		return variant;
	}

	template<typename T>
	inline VARIANT number_to_variant(T value)
	{
		VARIANT variant;
		VariantInit(&variant);
		if (typeid(T) == typeid(INT))
		{
			variant.vt = VT_INT;
			variant.intVal = value;
		}
		else
		{
			variant.vt = VT_I4;
			variant.iVal = value;
		}
		//VariantClear(&variant);
		return variant;
	}
}