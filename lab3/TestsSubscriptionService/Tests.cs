// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TestsSubscriptionService;

using System;
using System.Net;
using Moq;
using Subscription;

public class SubscriptionServiceTests
{
    private SubscriptionService _subscriptionService;

    //FIXED: вынес инициализацию объектов из каждого отдельного теста: сделал ее общей для всех тестов
    private Mock<IBenefitService> _benefitServiceStub = new Mock<IBenefitService>();
    private Mock<IEmailSender> _emailSenderStub = new Mock<IEmailSender>();

    public SubscriptionServiceTests()
    {
        _subscriptionService = new SubscriptionService(_benefitServiceStub.Object, _emailSenderStub.Object);
    }


    [Fact]
    public void GetBenefitsForCategory_returns_list_of_available_benefits()
    {
        //arrange
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        var firstBenefit = "Smart playlists, podcasts, and audiobooks";
        _benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                firstBenefit,
                "Exclusive releases and popular movies and shows",
                "Cashback as bonus points in services",
            });

        //act
        IEnumerable<string> benefits = _subscriptionService.GetAllBenefitsForCategory(entertainmentCategoryGuid);

        //FIXED: проверяю размер списка и его содержимое
        //assert
        Assert.Equal(3, benefits.Count());
        Assert.Equal(benefits.First(), firstBenefit);
    }

    [Fact]
    public void SearchBenefitFromCategory_returns_single_appropriate_benefit()
    {
        //arrange
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        var footballBenefit = "The Bundesliga, the Serie A, the Champions League, and more football tournaments";
        var footballBenefitKeyword = "football";
        _benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                "More movies and TV shows",
                footballBenefit,
                "Amediateka movies and TV shows",
            });

        //act
        string foundBenefit = _subscriptionService.SearchBenefitFromCategory(footballBenefitKeyword, entertainmentCategoryGuid);

        //assert
        Assert.Equal(footballBenefit, foundBenefit);
    }

    [Fact]
    public void SearchBenefitFromCategory_returns_first_found_benefit_when_several_approprtate_exist()
    {
        //arrange
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        var movieBenefit = "More movies and TV shows";
        _benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                movieBenefit,
                "The Bundesliga, the Serie A, the Champions League, and more football tournaments",
                "Amediateka movies and TV shows",
            });

        //act
        string foundBenefit = _subscriptionService.SearchBenefitFromCategory("movies", entertainmentCategoryGuid);

        //assert
        Assert.Equal(movieBenefit, foundBenefit);
    }

    [Fact]
    public void SearchBenefitFromCategory_returns_nothing_when_no_approprtate_benefit_exist()
    {
        //arrange
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        _benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>
            {
                "Books",
                "TV channels",
                "Streaming services",
            });

        //act
        string foundBenefit = _subscriptionService.SearchBenefitFromCategory("sport", entertainmentCategoryGuid);

        //assert
        Assert.Empty(foundBenefit);
    }

    [Fact]
    public void GetBenefitExpireDateTime_returns_dateTime()
    {
        //arrange
        Guid benefitGuid = Guid.NewGuid();
        DateTime expectedBenefitExpireDateTime = new DateTime(2022, 10, 5, 23, 59, 59);
        _benefitServiceStub
            .Setup(x => x.GetBenefitExpireDateTime(benefitGuid))
            .Returns(expectedBenefitExpireDateTime);

        //act
        var actualBenefitDateTime = _subscriptionService.GetBenefitExpireDateTime(benefitGuid);

        //assert
        Assert.Equal(expectedBenefitExpireDateTime, actualBenefitDateTime);
    }

    [Fact]
    public void Should_specify_sequential_start_and_end_subscription_periods()
    {
        //arrange
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 10, 4, 0, 59, 59);

        //Объединил два этапа как в примере https://stackoverflow.com/questions/45017295/assert-an-exception-using-xunit
        //act & assert
        Assert.Throws<ArgumentException>(() => _subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd));
    }

    [Fact]
    public void Should_specify_start_and_end_subscription_period_from_one_month()
    {
        //arrange
        var periodStart = new DateTime(2022, 10, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 10, 15, 0, 59, 59);

        //act & assert
        Assert.Throws<ArgumentException>(() => _subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd));
    }

    [Fact]
    public void Check_default_montly_cost_value()
    {
        //arrange
        var expectedMonthlyCost = 100;

        //act
        var monthlyCost = _subscriptionService.GetMonthlyCost();

        //assert
        Assert.Equal(monthlyCost, expectedMonthlyCost);
    }

    [Fact]
    public void Calculate_subscription_price_for_two_incomplete_months()
    {
        //arrange
        var periodStart = new DateTime(2022, 10, 5, 12, 58, 50);
        var periodEnd = new DateTime(2022, 12, 1, 0, 56, 42);
        var expectedPrice = _subscriptionService.GetMonthlyCost() * 2;

        //act
        var price = _subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd);

        //assert
        Assert.Equal(price, expectedPrice);
    }

    [Fact]
    public void Calculate_subscription_price_for_exactly_one_month()
    {
        //arrange
        var periodStart = new DateTime(2022, 1, 5, 23, 18, 11);
        var periodEnd = new DateTime(2022, 2, 5, 23, 18, 11);
        var oneMonthPrice = _subscriptionService.GetMonthlyCost();

        //act
        var actualPrice = _subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd);

        //assert
        Assert.Equal(actualPrice, oneMonthPrice);
    }

    [Fact]
    public void Calculate_subscription_price_for_almost_one_month()
    {
        //arrange
        var periodStart = new DateTime(2022, 5, 5, 23, 59, 59);
        var periodEnd = new DateTime(2022, 6, 5, 23, 59, 58);

        //act & assert
        Assert.Throws<ArgumentException>(() => _subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd));
    }

    [Fact]
    public void Add_additional_option_in_appropriate_format()
    {
        //arrange
        var firstAdditionalOption = new Tuple<string, string, decimal>("firstOption", "description", Convert.ToDecimal(50));

        //act
        _subscriptionService.CreateAdditionalOption(firstAdditionalOption.Item1, firstAdditionalOption.Item2, firstAdditionalOption.Item3);

        //assert
        Assert.Equal("firstOption: description", _subscriptionService.GetAdditionalOptions().First().Item2);
        Assert.Single(_subscriptionService.GetAdditionalOptions());  
    }

    [Fact]
    public void Add_several_additional_options()
    {
        //arrange
        var additionalOption1 = new Tuple<string, string, decimal>("option1", "description1", Convert.ToDecimal(60));
        var additionalOption2 = new Tuple<string, string, decimal>("option2", "description2", Convert.ToDecimal(70));

        //act
        _subscriptionService.CreateAdditionalOption(additionalOption1.Item1, additionalOption1.Item2, additionalOption1.Item3);
        _subscriptionService.CreateAdditionalOption(additionalOption2.Item1, additionalOption2.Item2, additionalOption2.Item3);

        //assert
        Assert.Equal(2, _subscriptionService.GetAdditionalOptions().Count());
        Assert.Equal($"{additionalOption1.Item1}: {additionalOption1.Item2}", _subscriptionService.GetAdditionalOptions().First().Item2);
    }

    [Fact]
    public void Remove_additional_option()
    {
        //arrange
        var guid = _subscriptionService.CreateAdditionalOption("option", "description", Convert.ToDecimal(100));

        //act
        _subscriptionService.RemoveAdditionalOption(guid);

        //assert
        Assert.Empty(_subscriptionService.GetAdditionalOptions());
    }

    [Fact]
    public void Remove_specified_not_single_additional_option()
    {
        //arrange
        var additionalOption1 = new Tuple<string, string, decimal>("addOption1", "description1", Convert.ToDecimal(10));
        var additionalOption2 = new Tuple<string, string, decimal>("addOption2", "description2", Convert.ToDecimal(20));
        var additionalOption3 = new Tuple<string, string, decimal>("addOption3", "description3", Convert.ToDecimal(30));
        var guidToRemove = _subscriptionService.CreateAdditionalOption(additionalOption1.Item1, additionalOption1.Item2, additionalOption1.Item3);
        _subscriptionService.CreateAdditionalOption(additionalOption2.Item1, additionalOption2.Item2, additionalOption2.Item3);
        var remainingGuid = _subscriptionService.CreateAdditionalOption(additionalOption3.Item1, additionalOption3.Item2, additionalOption3.Item3);

        //act
        _subscriptionService.RemoveAdditionalOption(guidToRemove);

        //assert
        Assert.NotEmpty(_subscriptionService.GetAdditionalOptions());
        //FIXED: обращаюсь по определенному индексу
        Assert.Equal(_subscriptionService.GetAdditionalOptions().ToList()[1].Item1, remainingGuid);
        Assert.Equal("addOption3: description3", _subscriptionService.GetAdditionalOptions().ToList()[1].Item2);
    }

    [Fact]
    public void Remove_from_empty_additional_option_storage()
    {
        //arrange
        var foreignGuid = Guid.NewGuid();

        //act
        _subscriptionService.RemoveAdditionalOption(foreignGuid);

        //assert
        Assert.Empty(_subscriptionService.GetAdditionalOptions());
    }

    [Fact]
    public void Change_monthly_cost_value()
    {
        //arrange
        var expectedNewMonthlyCost = 200;

        //act
        _subscriptionService.SetMonthlyCost(200);

        //arrange
        Assert.Equal(expectedNewMonthlyCost, _subscriptionService.GetMonthlyCost());
    }

    [Fact]
    public void Check_email_content_with_basic_notification_and_wrong_email_recipent()
    {
        //arrange
        var mailboxUnavailableStatusCode = 550;
        var emptyRecipentAddress = "";
        var subject = "Subscription service updates";
        var body = "Hi,\nnotification.\nSincerely, the service team.";
        _emailSenderStub
            .Setup(x => x.SendEmail(emptyRecipentAddress, subject, body))
            .Returns(mailboxUnavailableStatusCode);

        //act
        var statusCode = _subscriptionService.SendEmail(emptyRecipentAddress, "notification");

        //arrange
        Assert.Equal(statusCode, mailboxUnavailableStatusCode);
    }

    [Fact]
    public void Check_email_content_with_basic_notification()
    {
        //arrange
        var recipentAddress = "test@mail.com";
        var subject = "Subscription service updates";
        var body = "Hi,\nnotification.\nSincerely, the service team.";
        _emailSenderStub
            .Setup(x => x.SendEmail(recipentAddress, subject, body))
            .Returns((int)HttpStatusCode.OK);

        //act
        var statusCode = _subscriptionService.SendEmail(recipentAddress, "notification");

        //arrange
        Assert.Equal(statusCode, (int)HttpStatusCode.OK);
    }
}
