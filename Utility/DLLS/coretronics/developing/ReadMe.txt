修改項目:

	//新增ProjCompType，0為綠色鏡片，1紅色鏡片
        [DllImport(DLL_PATH, EntryPoint = "setCenterCompImg", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setCenterCompImg(int cols, int rows, int channels, IntPtr data, int ProjCompType);

新增項目：
	
	//中心座標、矩形角度(先假設為方正矩形暫為預留先不使用)、與寬高，後續於UI上顯示中心位置、矩形的四個位置(中心座標加減長寬各半距離)
	//回傳內容為Center.x,Center.y,Angle,Width,Height
        [DllImport(DLL_PATH, EntryPoint = "getCenterInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getCenterInfo(int ProjCompType, [MarshalAs(UnmanagedType.LPArray)] float[] CenterInfo);

	//回傳內容為Center.x,Center.y,Angle,Width,Height
        [DllImport(DLL_PATH, EntryPoint = "getGBProjInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getGBProjInfo([MarshalAs(UnmanagedType.LPArray)] float[] DefectInfo);

	//Center1.x,Center1.y,Angle1,Width1,Height1,Center2.x,Center2.y,Angle2,Width2,Height2,Center3.x,Center3.y(Midle Center)
        [DllImport(DLL_PATH, EntryPoint = "getMProjInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getMProjInfo([MarshalAs(UnmanagedType.LPArray)] float[] DefectInfo);


回傳內容修改：

	//回傳內容，從-1,0,1，改成回傳負像素差,0,正像素差，後續UI再根據距離差，給予三種不同移動距離做補償
        [DllImport(DLL_PATH, EntryPoint = "getProjCompInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void getProjCompInfo(int ProjCompType, [MarshalAs(UnmanagedType.LPArray)] int[] MotorParams);	