﻿using System.Threading.Tasks;
using Volo.Abp.Aspects;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace Volo.Abp.Features
{
    public class FeatureInterceptor : AbpInterceptor, ITransientDependency
    {
        private readonly IMethodInvocationFeatureCheckerService _methodInvocationFeatureCheckerService;

        public FeatureInterceptor(
            IMethodInvocationFeatureCheckerService methodInvocationFeatureCheckerService)
        {
            _methodInvocationFeatureCheckerService = methodInvocationFeatureCheckerService;
        }

        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (AbpCrossCuttingConcerns.IsApplied(invocation.TargetObject, AbpCrossCuttingConcerns.FeatureChecking))
            {
                await invocation.ProceedAsync().ConfigureAwait(false);
                return;
            }

            await CheckFeaturesAsync(invocation).ConfigureAwait(false);
            await invocation.ProceedAsync().ConfigureAwait(false);
        }

        protected virtual async Task CheckFeaturesAsync(IAbpMethodInvocation invocation)
        {
            await _methodInvocationFeatureCheckerService.CheckAsync(
                new MethodInvocationFeatureCheckerContext(
                    invocation.Method
                )
            ).ConfigureAwait(false);
        }
    }
}
