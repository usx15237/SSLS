using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using System.Web.Mvc;
using SSLS.Domain.Abstract;
using SSLS.Domain.Concrete;
using SSLS.WebUI.Infrastructure.Abstract;
using SSLS.WebUI.Infrastructure.Concrete;

namespace SSLS.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel Kernel;
        public NinjectDependencyResolver(IKernel KernelParam)
        {
            //创建一个Ninject内核的实例
            Kernel = KernelParam;
            AddBindings(); 
        }

        /*
         *  接口设置为Bind方法的类型参数，希望实例化的实现类设置为To方法的类型参数;二者由此相沟通         *  该语句告诉Ninject：当要求它实现IBooksReopository接口时，应当创建DatabaseOrderProcessor类的新实例。         
         */
        private void AddBindings()
        {
            EmailSettings emailSettings = new EmailSettings();
            Kernel.Bind<MailProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);
            Kernel.Bind<IBooksReopository>().To<EFBookRepository>();
            Kernel.Bind<IOrderProcessor>().To<DatabaseOrderProcessor>();
            Kernel.Bind<IAuthProvider>().To<FormAuthProvider>();
        }
        public object GetService(Type serviceType)
        {
            return Kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Kernel.GetAll(serviceType);
        }
    }
}