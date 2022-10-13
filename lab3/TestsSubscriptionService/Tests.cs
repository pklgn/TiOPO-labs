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
        var firstBenefit = "Smart playlists, podcasts, and audiobooks";
        benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                firstBenefit,
                "Exclusive releases and popular movies and shows",
                "Cashback as bonus points in services",
            });
        var subscriptionService = new SubscriptionService(benefitServiceStub.Object, null);

        IEnumerable<string> benefits = subscriptionService.GetAllBenefitsForCategory(entertainmentCategoryGuid);

        Assert.Equal(3, benefits.Count());
        Assert.Equal(benefits.First(), firstBenefit);
    }

    [Fact]
    public void SearchBenefitFromCategory_returns_single_appropriate_benefit()
    {
        var benefitServiceStub = new Mock<IBenefitService>();
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        var footballBenefit = "The Bundesliga, the Serie A, the Champions League, and more football tournaments";
        benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                "More movies and TV shows",
                footballBenefit,
                "Amediateka movies and TV shows",
            });
        var subscriptionService = new SubscriptionService(benefitServiceStub.Object, null);

        string foundBenefit = subscriptionService.SearchBenefitFromCategory("football", entertainmentCategoryGuid);

        Assert.Equal(footballBenefit, foundBenefit);
    }

    [Fact]
    public void SearchBenefitFromCategory_returns_first_found_benefit_when_several_approprtate_exist()
    {
        var benefitServiceStub = new Mock<IBenefitService>();
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        var movieBenefit = "More movies and TV shows";
        benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                movieBenefit,
                "The Bundesliga, the Serie A, the Champions League, and more football tournaments",
                "Amediateka movies and TV shows",
            });
        var subscriptionService = new SubscriptionService(benefitServiceStub.Object, null);

        string foundBenefit = subscriptionService.SearchBenefitFromCategory("movies", entertainmentCategoryGuid);

        Assert.Equal(movieBenefit, foundBenefit);
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

        Assert.Empty(foundBenefit);
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

        Assert.Throws<ArgumentException>(() => subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd));
    }

    [Fact]
    public void Should_specify_start_and_end_subscription_period_from_one_month()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 10, 15, 0, 59, 59);

        Assert.Throws<Exception>(() => subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd));
    }

    [Fact]
    public void Calculate_subscription_price_for_two_incomplete_months()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 12, 1, 0, 59, 59);
        var expectedPrice = 200;

        var price = subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd);

        Assert.Equal(price, expectedPrice);
    }

    [Fact]
    public void Calculate_subscription_price_for_exactly_one_month()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 11, 5, 23, 59, 59);
        var expectedPrice = 100;

        var price = subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd);

        Assert.Equal(price, expectedPrice);
    }

    [Fact]
    public void Calculate_subscription_price_for_almost_one_month()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 11, 5, 23, 59, 58);

        Assert.Throws<Exception>(() => subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd));
    }

    [Fact]
    public void Add_additional_option_in_appropriate_format()
    {
        var subscriptionService = new SubscriptionService(null, null);

        subscriptionService.CreateAdditionalOption("option1", "description", Convert.ToDecimal(50));

        Assert.Equal("option1: description", subscriptionService.GetAdditionalOptions().First().Item2);
        Assert.Single(subscriptionService.GetAdditionalOptions());
    }

    [Fact]
    public void Add_several_additional_options()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var additionalOption1 = new Tuple<string, string, decimal>("option1", "description1", Convert.ToDecimal(50));
        var additionalOption2 = new Tuple<string, string, decimal>("option2", "description2", Convert.ToDecimal(60));
        subscriptionService.CreateAdditionalOption(additionalOption1.Item1, additionalOption1.Item2, additionalOption1.Item3);
        subscriptionService.CreateAdditionalOption(additionalOption2.Item1, additionalOption2.Item2, additionalOption2.Item3);

        Assert.Equal(2, subscriptionService.GetAdditionalOptions().Count());
        Assert.Equal($"{additionalOption1.Item1}: {additionalOption1.Item2}", subscriptionService.GetAdditionalOptions().First().Item2);
    }

    [Fact]
    public void Remove_additional_option()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var guid = subscriptionService.CreateAdditionalOption("option1", "description", Convert.ToDecimal(50));

        subscriptionService.RemoveAdditionalOption(guid);

        Assert.Empty(subscriptionService.GetAdditionalOptions());
    }

    [Fact]
    public void Remove_specified_not_single_additional_option()
    {
        var subscriptionService = new SubscriptionService(null, null);
        var additionalOption1 = new Tuple<string, string, decimal>("addOption1", "description1", Convert.ToDecimal(50));
        var additionalOption2 = new Tuple<string, string, decimal>("addOption2", "description2", Convert.ToDecimal(50));
        var guidNotRemove = subscriptionService.CreateAdditionalOption(additionalOption1.Item1, additionalOption1.Item2, additionalOption1.Item3);
        var guidToRemove = subscriptionService.CreateAdditionalOption(additionalOption2.Item1, additionalOption2.Item2, additionalOption2.Item3);

        subscriptionService.RemoveAdditionalOption(guidToRemove);

        Assert.NotEmpty(subscriptionService.GetAdditionalOptions());
        Assert.Equal(subscriptionService.GetAdditionalOptions().First().Item1, guidNotRemove);
    }


}
