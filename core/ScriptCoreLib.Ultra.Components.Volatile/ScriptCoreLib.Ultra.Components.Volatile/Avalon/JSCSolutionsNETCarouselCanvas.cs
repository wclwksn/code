﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib.Ultra.Components.Avalon.Images;
using ScriptCoreLib.Ultra.Components.Volatile.Avalon.Images;

namespace ScriptCoreLib.Avalon
{
	public class JSCSolutionsNETCarouselCanvas : ImageCarouselCanvas
	{
		class JSCSolutionsNETArguments : Arguments
		{
			public JSCSolutionsNETArguments()
			{
				this.AddImages =
					Add =>
					{
                        // http://robert.ocallahan.org/2013/02/and-then-there-were-three.html

                        // iOS
                        //Add(new Apple_Safari());

                        // Chrome OS, Android
                        Add(new Google_Chrome());
                        Add(new Internet_Explorer_7_Logo());

                        Add(new Firefox_3_5_logo());
                        //Add(new Opera());
					};

                this.ImageDefaultWidth = JSCSolutionsNETImageOnWhite.ImageDefaultWidth;
                this.ImageDefaultHeight = JSCSolutionsNETImageOnWhite.ImageDefaultHeight;

				this.CreateCenterImage =
					() => new JSCSolutionsNETImageOnWhite();

			}
		}

		public JSCSolutionsNETCarouselCanvas()
			: base(new JSCSolutionsNETArguments())
		{

		}
	}
}
