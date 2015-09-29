using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Entities.DataContext;

namespace GitList.Core.Interfaces
{
    public interface IControllers
    {
        void Initialise();
        void Start();
    }
}
