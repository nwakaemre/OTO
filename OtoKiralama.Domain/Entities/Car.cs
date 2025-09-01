using System;
using OtoKiralama.Domain.Common;
using OtoKiralama.Domain.Enums;
using OtoKiralama.Domain.ValueObjects;

namespace OtoKiralama.Domain.Entities
{
    public class Car : AggregateRoot
    {
        public string Brand { get; private set; } = string.Empty;
        public string Model { get; private set; } = string.Empty;
        public int? Year { get; private set; }
        public FuelType FuelType { get; private set; }
        public TransmissionType TransmissionType { get; private set; }
        public Money DailyPrice { get; private set; }
        public Guid BranchId { get; private set; }
        public bool IsAvailable { get; private set; } = true;
        protected Car() { }

        private Car(
            Guid id,
            string brand,
            string model,
            int? year,
            FuelType fuelType,
            TransmissionType transmissionType,
            Money dailyPrice,
            Guid branchId)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id boş olamaz.", nameof(id));
            if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Brand boş olamaz.", nameof(brand));
            if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model boş olamaz.", nameof(model));
            if (branchId == Guid.Empty) throw new ArgumentException("BranchId boş olamaz.", nameof(branchId));
            if (dailyPrice.Amount < 0m) throw new ArgumentOutOfRangeException(nameof(dailyPrice), "Günlük fiyat negatif olamaz.");

            Id = id;
            Brand = brand.Trim();
            Model = model.Trim();
            Year = year;
            FuelType = fuelType;
            TransmissionType = transmissionType;
            DailyPrice = dailyPrice;
            BranchId = branchId;
        }

        public static Car Create(
            Guid id,
            string brand,
            string model,
            int? year,
            FuelType fuelType,
            TransmissionType transmissionType,
            Money dailyPrice,
            Guid branchId)
            => new(
                id,
                brand,
                model,
                year,
                fuelType,
                transmissionType,
                dailyPrice,
                branchId);

        public void SetDailyPrice(Money price)
        {
            if (price.Amount < 0m)
                throw new ArgumentOutOfRangeException(nameof(price), "Günlük fiyat negatif olamaz.");

            DailyPrice = price;
            Touch();
        }

        public void ChangeBranch(Guid branchId)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("BranchId boş olamaz.", nameof(branchId));

            BranchId = branchId;
            Touch();
        }
        public void UpdateDetails(
            string brand,
            string model,
            int? year,
            FuelType fuelType,
            TransmissionType transmissionType)
        {
            if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Brand is required", nameof(brand));
            if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model is required", nameof(model));
            if (year is not null && (year < 1980 || year > DateTime.UtcNow.Year + 1))
                throw new ArgumentOutOfRangeException(nameof(year), "Year is out of valid range.");

            Brand = brand;
            Model = model;
            Year = year;
            FuelType = fuelType;
            TransmissionType = transmissionType;
        }
        // 🚦 Yeni methodlar
        public void MarkAsRented() => IsAvailable = false;
        public void MarkAsAvailable() => IsAvailable = true;
    }
}