using System;
using OtoKiralama.Domain.Common;
using OtoKiralama.Domain.Enums;
using OtoKiralama.Domain.Events;
using OtoKiralama.Domain.ValueObjects;

namespace OtoKiralama.Domain.Entities
{
    public class Reservation : AggregateRoot
    {
        public Guid CarId { get; private set; }
        public Guid CustomerId { get; private set; }
        public DateRange Period { get; private set; }
        public Money TotalPrice { get; private set; }
        public string? AppliedCouponCode { get; private set; }
        public string? AppliedInsuranceCode { get; private set; }
        public ReservationStatus Status { get; private set; }

        // Yeni alanlar
        public PaymentStatus PaymentStatus { get; private set; }
        public string? PaymentIntentId { get; private set; }
        public DateTimeOffset? ConfirmedAt { get; private set; }

        protected Reservation()
        {
            // CS8618 için TotalPrice'ı initialize et
            TotalPrice = Money.Zero();
        }

        private Reservation(Guid id, Guid carId, Guid customerId, DateRange period)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id boş olamaz.", nameof(id));
            if (carId == Guid.Empty) throw new ArgumentException("CarId boş olamaz.", nameof(carId));
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId boş olamaz.", nameof(customerId));

            Id = id;
            CarId = carId;
            CustomerId = customerId;
            Period = period;
            TotalPrice = Money.Zero();
            Status = ReservationStatus.Pending;

            // Varsayılanlar
            PaymentStatus = OtoKiralama.Domain.Enums.PaymentStatus.Pending;
            PaymentIntentId = null;
            ConfirmedAt = null;
        }

        public static Reservation Create(Guid id, Guid carId, Guid customerId, DateRange period)
            => new(id, carId, customerId, period);

        public Money CalculateTotal(Money dailyPrice)
        {
            var days = Period.LengthInDays();
            var total = dailyPrice.Multiply(days);
            TotalPrice = total;
            Touch();
            return total;
        }

        public void SetFinalPrice(Money total, string? appliedCouponCode, string? appliedInsuranceCode)
        {
            TotalPrice = total;
            AppliedCouponCode = appliedCouponCode;
            AppliedInsuranceCode = appliedInsuranceCode;
            Touch();
        }

        public void Confirm()
        {
            if (Status != ReservationStatus.Pending)
                throw new InvalidOperationException("Sadece Pending durumdayken Confirm yapılabilir.");

            Status = ReservationStatus.Confirmed;
            Touch();
        }

        public void Cancel()
        {
            if (Status is not ReservationStatus.Pending and not ReservationStatus.Confirmed)
                throw new InvalidOperationException("Sadece Pending veya Confirmed durumdayken Cancel yapılabilir.");

            Status = ReservationStatus.Cancelled;
            Touch();
        }

        public void Complete()
        {
            if (Status != ReservationStatus.Confirmed)
                throw new InvalidOperationException("Sadece Confirmed durumdayken Complete yapılabilir.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (today <= Period.End)
                throw new InvalidOperationException("Rezervasyon dönemi henüz bitmedi.");

            Status = ReservationStatus.Completed;
            Touch();
        }

        // Domain event kuyruğa ekleme (publish edilmez)
        public void EnqueueCreatedEvent()
            => AddDomainEvent(new ReservationCreatedEvent(Id, CarId, CustomerId, Period));

        public void SetPaymentStatus(PaymentStatus status)
        {
            PaymentStatus = status;
            Touch(); // opsiyonel: güncelleme zamanını güncellemek için
        }

        public void SetPaymentIntentId(string intentId)
        {
            PaymentIntentId = intentId;
        }

        public void SetConfirmedAt(DateTimeOffset? confirmedAt)
        {
            ConfirmedAt = confirmedAt;
            Touch();
        }
    }
}