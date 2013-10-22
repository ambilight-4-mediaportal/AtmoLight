// Copyright (C) 2005-2010 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#include "stdafx.h"


#include <d3d9.h>
#include <d3dx9.h>
#include <d3d9types.h>
#include <strsafe.h>

extern "C" __declspec(dllexport) HRESULT __stdcall VideoSurfaceToRGBSurfaceExt(IDirect3DSurface9* source,int sourceWidth,int sourceHeight, IDirect3DSurface9* dest,int destWidth,int destHeight)
{
	IDirect3DDevice9* device = NULL;
	HRESULT hr = source->GetDevice(&device);

	RECT sourceRect;
	sourceRect.left=0;
	sourceRect.right=sourceWidth;
	sourceRect.top=0;
  sourceRect.bottom=sourceHeight;

	RECT destRect;
	destRect.left=0;
	destRect.right=destWidth;
	destRect.top=0;
	destRect.bottom=destHeight;

	if(!FAILED(hr)){
		hr = device->StretchRect(source,&sourceRect,dest,&destRect,D3DTEXF_NONE);
	}
	//delete device;
	return hr;
}


#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif

