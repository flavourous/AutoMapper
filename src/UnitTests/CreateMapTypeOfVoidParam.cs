namespace AutoMapper.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Shouldly;

    namespace AutoMapper.UnitTests
    {
        public class CreateMapTypeOfVoidParam : AutoMapperSpecBase
        {
            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap(typeof(void), typeof(IContainer<>)).ConvertUsing(typeof(TCf<,>));
                cfg.CreateMap(typeof(IContainer<>), typeof(void)).ConvertUsing(typeof(TCb<,>));
            });
            interface IContainer<T> { T Value { get; set; } }
            class Container<T> : IContainer<T>
            {
                public Container() { }
                public Container(T val) { Value = val; }
                public T Value { get; set; }
            }
            [Fact]
            public void Maps_params_sucessful()
            {
                Mapper.Map<String>(new Container<String>("foo") as IContainer<String>).ShouldBe("foo");
                Mapper.Map<String>(new Container<String>() as IContainer<String>).ShouldBe(null);
                Mapper.Map<String, IContainer<String>>("foo", new Container<String>()).Value.ShouldBe("foo");
                Mapper.Map<String, IContainer<String>>(null, new Container<String>()).Value.ShouldBeNull();
                Mapper.Map<Container<String>>("foo").Value.ShouldBe("foo");
                Mapper.Map<Container<String>>(null)?.Value.ShouldBeNull();
                Mapper.Map<IContainer<String>>("foo").Value.ShouldBe("foo");
                Mapper.Map<IContainer<String>>(null)?.Value.ShouldBeNull();
                Mapper.Map<String, IContainer<String>>(null, new Container<String>("nobar")).Value.ShouldBeNull();
                Mapper.Map<IContainer<String>, String>(null, "nobar").ShouldBeNull();
            }
            class TCf<X, Y> : ITypeConverter<X, IContainer<Y>>
            {
                public IContainer<Y> Convert(X source, IContainer<Y> destination, ResolutionContext context)
                {
                    if (destination == null)
                        destination = new Container<Y>();
                    destination.Value = context.Mapper.Map<Y>(source);
                    return destination;
                }
            }
            class TCb<X, Y> : ITypeConverter<IContainer<X>, Y>
            {
                public Y Convert(IContainer<X> source, Y destination, ResolutionContext context)
                {
                    var use = source == null ? default(X) : source.Value;
                    return context.Mapper.Map<Y>(use);
                }
            }
        }
    }
}