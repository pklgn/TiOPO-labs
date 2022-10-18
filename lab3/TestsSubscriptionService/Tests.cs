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

    private Mock<IBenefitService> _benefitServiceStub = new Mock<IBenefitService>();
    private Mock<IEmailSender> _emailSenderStub = new Mock<IEmailSender>();

    public SubscriptionServiceTests()
    {
        _subscriptionService = new SubscriptionService(_benefitServiceStub.Object, _emailSenderStub.Object);
    }

    //FIXED: изменил названия тест-кейсов
    [Fact]
    public void Get_benefits_for_category_as_a_list_of_available_benefits()
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

        //FIXED: исправил порядок assert на expected-actual
        //assert
        Assert.Equal(3, benefits.Count());
        Assert.Equal(firstBenefit, benefits.First());
    }

    [Fact]
    public void Search_benefit_from_category_returns_single_appropriate_benefit()
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
        string? foundBenefit = _subscriptionService.SearchBenefitFromCategory(footballBenefitKeyword, entertainmentCategoryGuid);

        //assert
        Assert.Equal(footballBenefit, foundBenefit);
    }

    [Fact]
    public void Search_benefit_from_category_returns_first_found_benefit_when_several_approprtate_exist()
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
        string? foundBenefit = _subscriptionService.SearchBenefitFromCategory("movies", entertainmentCategoryGuid);

        //assert
        Assert.Equal(movieBenefit, foundBenefit);
    }

    [Fact]
    public void Search_benefit_from_category_returns_nothing_when_no_approprtate_benefit_exist()
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
        string? foundBenefit = _subscriptionService.SearchBenefitFromCategory("sport", entertainmentCategoryGuid);

        //assert
        Assert.Null(foundBenefit);
    }

    //FIXED: добавил поиск в пустом списке
    [Fact]
    public void Search_benefit_from_category_in_empty_list()
    {
        //arrange
        Guid entertainmentCategoryGuid = Guid.NewGuid();
        _benefitServiceStub
            .Setup(x => x.GetBenefitsForCategory(entertainmentCategoryGuid))
            .Returns(new List<string>{});

        //act
        string? foundBenefit = _subscriptionService.SearchBenefitFromCategory("sport", entertainmentCategoryGuid);

        //assert
        Assert.Null(foundBenefit);
    }

    [Fact]
    public void Get_benefit_expire_datetime_returns_datetime()
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
        var actualMonthlyCost = _subscriptionService.GetMonthlyCost();

        //assert
        Assert.Equal(expectedMonthlyCost, actualMonthlyCost);
    }

    [Fact]
    public void Calculate_subscription_price_for_two_incomplete_months()
    {
        //arrange
        var periodStart = new DateTime(2022, 10, 5, 12, 58, 50);
        var periodEnd = new DateTime(2022, 12, 1, 0, 56, 42);
        var expectedPrice = 200;

        //act
        var actualPrice = _subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd);

        //assert
        Assert.Equal(expectedPrice, actualPrice);
    }

    [Fact]
    public void Calculate_subscription_price_for_exactly_one_month()
    {
        //arrange
        var periodStart = new DateTime(2022, 1, 5, 23, 18, 11);
        var periodEnd = new DateTime(2022, 2, 5, 23, 18, 11);
        //FIXED заменил использование GetMonthlyCost на константу
        var oneMonthPrice = 100;

        //act
        var actualPrice = _subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd);

        //assert
        Assert.Equal(oneMonthPrice, actualPrice);
    }

    [Fact]
    public void Calculate_subscription_price_for_one_year()
    {
        //arrange
        var periodStart = new DateTime(2022, 1, 5, 23, 18, 11);
        var periodEnd = new DateTime(2023, 2, 5, 23, 18, 11);
        //FIXED заменил использование GetMonthlyCost на константу
        var expectedPrice = 1300;

        //act
        var actualPrice = _subscriptionService.CalculateDateTimeRangeCost(periodStart, periodEnd);

        //assert
        Assert.Equal(expectedPrice, actualPrice);
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
        //FIXED: переименовал из Item1, Item2, Item3
        (string name, string desc, decimal cost) additionalOption = ("firstOption", "description", Convert.ToDecimal(50));

        //act
        _subscriptionService.CreateAdditionalOption(additionalOption.name, additionalOption.desc, additionalOption.cost);

        //assert
        Assert.Equal("firstOption: description", _subscriptionService.GetAdditionalOptions().First().Item2);
        Assert.Single(_subscriptionService.GetAdditionalOptions());  
    }

    [Fact]
    public void Add_several_additional_options()
    {
        //arrange
        (string name, string desc, decimal cost) additionalOption1 = ("option1", "description1", Convert.ToDecimal(60));
        (string name, string desc, decimal cost) additionalOption2 = ("option2", "description2", Convert.ToDecimal(70));

        //act
        var guid1 = _subscriptionService.CreateAdditionalOption(additionalOption1.name, additionalOption1.desc, additionalOption1.cost);
        _subscriptionService.CreateAdditionalOption(additionalOption2.name, additionalOption2.desc, additionalOption2.cost);

        //assert
        Assert.Equal(2, _subscriptionService.GetAdditionalOptions().Count());
        Assert.Equal($"{additionalOption1.Item1}: {additionalOption1.Item2}", _subscriptionService.GetAdditionalOptions().First().Item2);
        //FIXED: добавил проверку для Item1
        Assert.Equal(guid1, _subscriptionService.GetAdditionalOptions().First().Item1);
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
        (string name, string desc, decimal cost) additionalOption1 = ("addOption1", "description1", Convert.ToDecimal(10));
        (string name, string desc, decimal cost) additionalOption2 = ("addOption2", "description2", Convert.ToDecimal(20));
        (string name, string desc, decimal cost) additionalOption3= ("addOption3", "description3", Convert.ToDecimal(30));
        var guidToRemove = _subscriptionService.CreateAdditionalOption(additionalOption1.name, additionalOption1.desc, additionalOption1.cost);
        var remainingGuid2 = _subscriptionService.CreateAdditionalOption(additionalOption2.name, additionalOption2.desc, additionalOption2.cost);
        var remainingGuid3 = _subscriptionService.CreateAdditionalOption(additionalOption3.name, additionalOption3.desc, additionalOption3.cost);

        //act
        _subscriptionService.RemoveAdditionalOption(guidToRemove);

        //assert
        Assert.NotEmpty(_subscriptionService.GetAdditionalOptions());
        Assert.Equal(remainingGuid3, _subscriptionService.GetAdditionalOptions().ToList()[1].Item1);
        Assert.Equal("addOption3: description3", _subscriptionService.GetAdditionalOptions().ToList()[1].Item2);
        //FIXED: добавил проверку для второго оставшегося элемента
        Assert.Equal(remainingGuid2, _subscriptionService.GetAdditionalOptions().ToList()[0].Item1);
        Assert.Equal("addOption2: description2", _subscriptionService.GetAdditionalOptions().ToList()[0].Item2);
    }

    [Fact]
    public void Remove_from_empty_additional_option_storage()
    {
        //arrange
        var foreignGuid = Guid.NewGuid();

        //act & assert
        //FIXED: бросаю исключение при попытке удаления несуществующего элемента
        Assert.Throws<ArgumentException>(() => _subscriptionService.RemoveAdditionalOption(foreignGuid));
    }

    [Fact]
    public void Remove_not_existing_element_from_additional_list()
    {
        //arrange
        var foreignGuid = Guid.NewGuid();
        (string name, string desc, decimal cost) additionalOption = ("addOption1", "description1", Convert.ToDecimal(1000));

        //act & assert
        //FIXED: бросаю исключение при попытке удаления несуществующего элемента
        Assert.Throws<ArgumentException>(() => _subscriptionService.RemoveAdditionalOption(foreignGuid));
    }

    [Fact]
    public void Change_monthly_cost_value()
    {
        //arrange
        var expectedNewMonthlyCost = 200;

        //act
        //FIXED: использую константу
        _subscriptionService.SetMonthlyCost(expectedNewMonthlyCost);

        //assert
        Assert.Equal(expectedNewMonthlyCost, _subscriptionService.GetMonthlyCost());
    }

    [Fact]
    public void Check_email_content_with_basic_notification_and_wrong_email_recipent()
    {
        //arrange
        var mailboxUnavailableStatusCode = 550;
        var emptyRecipentAddress = "";
        //FIXED: заменил конкретные переменные на It.IsAny<string>()
        _emailSenderStub
            .Setup(x => x.SendEmail(emptyRecipentAddress, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(mailboxUnavailableStatusCode);

        //act
        var actualStatusCode = _subscriptionService.SendEmail(emptyRecipentAddress, "notification");

        //assert
        Assert.Equal(mailboxUnavailableStatusCode, actualStatusCode);
    }

    [Fact]
    public void Check_email_content_with_basic_notification()
    {
        //arrange
        var recipentAddress = "test@mail.com";
        _emailSenderStub
            .Setup(x => x.SendEmail(recipentAddress, It.IsAny<string>(), It.IsAny<string>()))
            .Returns((int)HttpStatusCode.OK);

        //act
        var actualStatusCode = _subscriptionService.SendEmail(recipentAddress, "notification");

        //assert
        Assert.Equal((int)HttpStatusCode.OK, actualStatusCode);
    }
}
