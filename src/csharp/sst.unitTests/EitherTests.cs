using NUnit.Framework;
using QResurgence.SST.Extensions;
using QResurgence.SST.Utilities;

namespace QResurgence.SST.UnitTests
{
    public class EitherTests
    {
        [Test]
        public void MapRightTest()
        {
            var rightContainer = new Right<LeftFirstType, RightFirstType>(new RightFirstType());

            var mapHandlerExecuted = false;
            var foldHandleExecuted = false;

            rightContainer
                .Map(_ =>
                {
                    mapHandlerExecuted = true;
                    return new RightSecondType();
                })
                .Fold(_ => foldHandleExecuted = true);

            Assert.True(mapHandlerExecuted);
            Assert.False(foldHandleExecuted);
        }

        [Test]
        public void MapLeftTest()
        {
            var leftContainer = new Left<LeftFirstType, RightFirstType>(new LeftFirstType());

            var mapHandlerExecuted = false;
            var foldHandleExecuted = false;

            leftContainer
                .Map(_ =>
                {
                    mapHandlerExecuted = true;
                    return new RightSecondType();
                })
                .Fold(_ => foldHandleExecuted = true);

            Assert.False(mapHandlerExecuted);
            Assert.True(foldHandleExecuted);
        }

        [Test]
        public void MapReturningEitherRightTest()
        {
            var rightContainer = new Right<LeftFirstType, RightFirstType>(new RightFirstType());

            var mapHandlerExecuted = false;
            var foldHandleExecuted = false;

            rightContainer
                .Map(_ =>
                {
                    mapHandlerExecuted = true;
                    return (Either<LeftFirstType, RightSecondType>) new Right<LeftFirstType, RightSecondType>(
                        new RightSecondType());
                })
                .Fold(_ => foldHandleExecuted = true);

            Assert.True(mapHandlerExecuted);
            Assert.False(foldHandleExecuted);
        }

        [Test]
        public void MapReturningEitherRightFoldTest()
        {
            var leftContainer = new Left<LeftFirstType, RightFirstType>(new LeftFirstType());

            var mapHandlerExecuted = false;
            var foldHandleExecuted = false;

            leftContainer
                .Map(_ =>
                {
                    mapHandlerExecuted = true;
                    return (Either<LeftFirstType, RightSecondType>) new Right<LeftFirstType, RightSecondType>(
                        new RightSecondType());
                })
                .Fold(_ => foldHandleExecuted = true);

            Assert.False(mapHandlerExecuted);
            Assert.True(foldHandleExecuted);
        }

        [Test]
        public void MapReturningEitherLeftTest()
        {
            var rightContainer = new Right<LeftFirstType, RightFirstType>(new RightFirstType());

            var mapHandlerExecuted = false;
            var foldHandleExecuted = false;

            rightContainer
                .Map(_ =>
                {
                    mapHandlerExecuted = true;
                    return (Either<LeftFirstType, RightSecondType>) new Left<LeftFirstType, RightSecondType>(
                        new LeftFirstType());
                })
                .Fold(_ => foldHandleExecuted = true);

            Assert.True(mapHandlerExecuted);
            Assert.True(foldHandleExecuted);
        }

        [Test]
        public void ConditionalMapPredicateSatisfiedTest()
        {
            var rightContainer = new Right<LeftFirstType, RightFirstType>(new RightFirstType());

            var satisfiedHandlerExecuted = false;
            var notSatisfiedHandlerExecuted = false;
            var foldHandleExecuted = false;

            rightContainer
                .Map(_ =>
                    {
                        satisfiedHandlerExecuted = true;
                        return new RightSecondType();
                    }, () =>
                    {
                        notSatisfiedHandlerExecuted = true;
                        return new LeftFirstType();
                    },
                    _ => true)
                .Fold(_ => foldHandleExecuted = true);

            Assert.True(satisfiedHandlerExecuted);
            Assert.False(notSatisfiedHandlerExecuted);
            Assert.False(foldHandleExecuted);
        }

        [Test]
        public void ConditionalMapPredicateNotSatisfiedTest()
        {
            var rightContainer = new Right<LeftFirstType, RightFirstType>(new RightFirstType());

            var satisfiedHandlerExecuted = false;
            var notSatisfiedHandlerExecuted = false;
            var foldHandleExecuted = false;

            rightContainer
                .Map(_ =>
                    {
                        satisfiedHandlerExecuted = true;
                        return new RightSecondType();
                    }, () =>
                    {
                        notSatisfiedHandlerExecuted = true;
                        return new LeftFirstType();
                    },
                    _ => false)
                .Fold(_ => foldHandleExecuted = true);

            Assert.False(satisfiedHandlerExecuted);
            Assert.True(notSatisfiedHandlerExecuted);
            Assert.True(foldHandleExecuted);
        }

        [Test]
        public void ConditionalMapFoldTest()
        {
            var leftContainer = new Left<LeftFirstType, RightFirstType>(new LeftFirstType());

            var satisfiedHandlerExecuted = false;
            var notSatisfiedHandlerExecuted = false;
            var foldHandleExecuted = false;

            leftContainer
                .Map(_ =>
                    {
                        satisfiedHandlerExecuted = true;
                        return new RightSecondType();
                    }, () =>
                    {
                        notSatisfiedHandlerExecuted = true;
                        return new LeftFirstType();
                    },
                    _ => false)
                .Fold(_ => foldHandleExecuted = true);

            Assert.False(satisfiedHandlerExecuted);
            Assert.False(notSatisfiedHandlerExecuted);
            Assert.True(foldHandleExecuted);
        }

        [Test]
        public void CaseTest()
        {
            var rightFirstContainer = new Right<LeftFirstType, RightFirstType>(new RightFirstType());
            var rightSecondContainer = new Right<LeftFirstType, RightSecondType>(new RightSecondType());

            var caseFirstLeftHandler = false;
            var caseSecondLeftHandler = false;
            var foldHandleExecuted = false;

            rightFirstContainer
                .Case((RightFirstType r) => caseFirstLeftHandler = true)
                .Case((RightSecondType r) => caseSecondLeftHandler = true)
                .Fold(_ => foldHandleExecuted = true);

            Assert.True(caseFirstLeftHandler);
            Assert.False(caseSecondLeftHandler);
            Assert.False(foldHandleExecuted);

            caseFirstLeftHandler = false;
            caseSecondLeftHandler = false;
            foldHandleExecuted = false;

            rightSecondContainer
                .Case((RightFirstType r) => caseFirstLeftHandler = true)
                .Case((RightSecondType r) => caseSecondLeftHandler = true)
                .Fold(_ => foldHandleExecuted = true);

            Assert.False(caseFirstLeftHandler);
            Assert.True(caseSecondLeftHandler);
            Assert.False(foldHandleExecuted);
        }

        [Test]
        public void CaseFoldTest()
        {
            var leftContainer = new Left<LeftFirstType, RightFirstType>(new LeftFirstType());

            var caseFirstLeftHandler = false;
            var caseSecondLeftHandler = false;
            var foldHandleExecuted = false;

            leftContainer
                .Case((RightFirstType r) => caseFirstLeftHandler = true)
                .Case((RightSecondType r) => caseSecondLeftHandler = true)
                .Fold(_ => foldHandleExecuted = true);

            Assert.False(caseFirstLeftHandler);
            Assert.False(caseSecondLeftHandler);
            Assert.True(foldHandleExecuted);
        }

        [Test]
        public void FoldTest()
        {
            var leftContainer = new Left<LeftFirstType, RightFirstType>(new LeftFirstType());

            var foldFirstHandlerExecuted = false;
            var foldSecondHandlerExecuted = false;
            var foldFinalHandlerExecuted = false;

            leftContainer
                // ReSharper disable once IsExpressionAlwaysTrue
                .Fold(_ => foldFirstHandlerExecuted = true, f => f is LeftFirstType)
#pragma warning disable 184
                .Fold(_ => foldSecondHandlerExecuted = true, f => f is LeftSecondType)
#pragma warning restore 184
                .Fold(_ => foldFinalHandlerExecuted = true);

            Assert.True(foldFirstHandlerExecuted);
            Assert.False(foldSecondHandlerExecuted);
            Assert.True(foldFinalHandlerExecuted);
        }

        internal class LeftFirstType
        {
        }

        internal class LeftSecondType
        {
        }

        internal class RightFirstType
        {
        }

        internal class RightSecondType
        {
        }
    }
}