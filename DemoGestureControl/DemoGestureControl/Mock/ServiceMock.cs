using DemoGestureControl.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGestureControl.Mock
{
    public class ServiceMock
    {

        public List<ImageComponent> GetListCategoryComponents()
        {
            List<ImageComponent> images = new List<ImageComponent>();

            ImageComponent firsImageComponent = new ImageComponent();
            firsImageComponent.ImageComponetId = 1;
            firsImageComponent.Name = "culturaTihuanaco";
            firsImageComponent.Url = "Images/image01.jpg";
            firsImageComponent.Type = 1;
            images.Add(firsImageComponent);


            ImageComponent secondImageComponent = new ImageComponent();
            secondImageComponent.ImageComponetId = 2;
            secondImageComponent.Name = "regionArequipa";
            secondImageComponent.Url = "Images/image02.jpg";
            secondImageComponent.Type = 1;
            images.Add(secondImageComponent);

            ImageComponent thirdImageComponent = new ImageComponent();
            thirdImageComponent.ImageComponetId = 3;
            thirdImageComponent.Name = "machupicchu";
            thirdImageComponent.Url = "Images/image03.jpg";
            thirdImageComponent.Type = 1;
            images.Add(thirdImageComponent);

            return images;
        }
    }
}
