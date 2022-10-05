// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TestsSubscriptionService;

using System;
using Moq;
using Subscription;

public class SubscriptionServiceChecking
{
    [Fact]
    public void GetBenefitsForCategory_returns_list_of_available_benefits()
    {
        var benefitServiceStub = new Mock<IBenefitService>();
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                "Smart playlists, podcasts, and audiobooks",
                "Exclusive releases and popular movies and shows",
                "Cashback as bonus points in services",
            });

        var subscriptionService = new SubscriptionService(benefitServiceStub.Object, null);

        IEnumerable<string> benefits = subscriptionService.GetAllBenefitsForCategory(entertainmentCategoryGuid);

        Assert.Equal(3, benefits.Count());
    }

    [Fact]
    public void SearchBenefitFromCategory_returns_single_appropriate_benefit()
    {
        var benefitServiceStub = new Mock<IBenefitService>();
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                "More movies and TV shows",
                "The Bundesliga, the Serie A, the Champions League, and more football tournaments",
                "Amediateka movies and TV shows",
            });

        var subscriptionService = new SubscriptionService(benefitServiceStub.Object, null);

        string foundBenefit = subscriptionService.SearchBenefitFromCategory("football", entertainmentCategoryGuid);

        Assert.Equal("The Bundesliga, the Serie A, the Champions League, and more football tournaments", foundBenefit);
    }

    [Fact]
    public void SearchBenefitFromCategory_returns_first_found_benefit_when_several_approprtate_exist()
    {
        var benefitServiceStub = new Mock<IBenefitService>();
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                "More movies and TV shows",
                "The Bundesliga, the Serie A, the Champions League, and more football tournaments",
                "Amediateka movies and TV shows",
            });

        var subscriptionService = new SubscriptionService(benefitServiceStub.Object, null);

        string foundBenefit = subscriptionService.SearchBenefitFromCategory("movies", entertainmentCategoryGuid);

        Assert.Equal("More movies and TV shows", foundBenefit);
    }

    [Fact]
    public void SearchBenefitFromCategory_returns_nothing_when_no_approprtate_benefit_exist()
    {
        var benefitServiceStub = new Mock<IBenefitService>();
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                "Books",
                "TV channels",
                "Streaming services",
            });

        var subscriptionService = new SubscriptionService(benefitServiceStub.Object, null);

        string foundBenefit = subscriptionService.SearchBenefitFromCategory("sport", entertainmentCategoryGuid);

        Assert.Equal("Benefit for specifiend keyword wasn't found\n", foundBenefit);
    }

    [Fact]
    public void GetBenefitExpireDateTime_returns_dateTime()
    {
        var benefitServiceStub = new Mock<IBenefitService>();
        Guid benefitGuid = Guid.NewGuid();
        DateTime expectedBenefitExpireDateTime = new DateTime(2022, 10, 5, 23, 59, 59);
        benefitServiceStub
            .Setup(x => x.GetBenefitExpireDateTime(benefitGuid))
            .Returns(expectedBenefitExpireDateTime);

        var subscriptionService = new SubscriptionService(benefitServiceStub.Object, null);

        var actualBenefitDateTime = subscriptionService.GetBenefitExpireDateTime(benefitGuid);

        Assert.Equal(expectedBenefitExpireDateTime, actualBenefitDateTime);
    }

    [Fact]
    public void Should_specify_sequential_start_and_end_subscription_periods()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 10, 4, 0, 59, 59);

        Assert.Throws<ArgumentException>(() => subscriptionService.CalculatePriceForPeriod(periodStart, periodEnd));
    }

    [Fact]
    public void Should_specify_start_and_end_subscription_period_from_one_month()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 10, 15, 0, 59, 59);

        Assert.Throws<Exception>(() => subscriptionService.CalculatePriceForPeriod(periodStart, periodEnd));
    }

    [Fact]
    public void Calculate_subscription_price_for_two_incomplete_months()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 12, 1, 0, 59, 59);
        var expectedPrice = 200;

        var price = subscriptionService.CalculatePriceForPeriod(periodStart, periodEnd);

        Assert.Equal(price, expectedPrice);
    }

    [Fact]
    public void Calculate_subscription_price_for_exactly_one_month()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 11, 5, 23, 59, 59);
        var expectedPrice = 100;

        var price = subscriptionService.CalculatePriceForPeriod(periodStart, periodEnd);

        Assert.Equal(price, expectedPrice);
    }

    [Fact]
    public void Add_additional_option_in_appropriate_format()
    {
        var subscriptionService = new SubscriptionService(null, null);

        subscriptionService.CreateAdditionOption("option1", "description");

        Assert.Equal("option1: description", subscriptionService.GetAdditionalOptions().First().Item2);
    }

    [Fact]
    public void Remove_additional_option()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var guid = subscriptionService.CreateAdditionOption("option1", "description");

        subscriptionService.RemoveAdditionOption(guid);

        Assert.Empty(subscriptionService.GetAdditionalOptions());
    }
}
