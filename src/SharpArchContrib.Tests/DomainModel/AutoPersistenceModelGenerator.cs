using System;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using SharpArch.Core.DomainModel;
using SharpArch.Data.NHibernate.FluentNHibernate;
using Tests.DomainModel.Conventions;
using Tests.DomainModel.Entities;

namespace Tests.DomainModel {
    public class AutoPersistenceModelGenerator : IAutoPersistenceModelGenerator {
        #region IAutoPersistenceModelGenerator Members

        public AutoPersistenceModel Generate() {
            var mappings = new AutoPersistenceModel();
            mappings.AddEntityAssembly(typeof (TestEntity).Assembly).Where(GetAutoMappingFilter);
            mappings.Conventions.Setup(GetConventions());
            mappings.Setup(GetSetup());
            mappings.IgnoreBase<Entity>();
            mappings.IgnoreBase(typeof (EntityWithTypedId<>));
            mappings.UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>();
            return mappings;
        }

        #endregion

        private Action<AutoMappingExpressions> GetSetup() {
            return c => { c.FindIdentity = type => type.Name == "Id"; };
        }

        private Action<IConventionFinder> GetConventions() {
            return c =>
                       {
                           c.Add<PrimaryKeyConvention>();
                           c.Add<ReferenceConvention>();
                           c.Add<HasManyConvention>();
                           c.Add<TableNameConvention>();
                       };
        }

        /// <summary>
        /// Provides a filter for only including types which inherit from the IEntityWithTypedId interface.
        /// </summary>
        private bool GetAutoMappingFilter(Type t) {
            return t.GetInterfaces().Any(x =>
                                         x.IsGenericType &&
                                         x.GetGenericTypeDefinition() == typeof (IEntityWithTypedId<>));
        }
    }
}